using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Exceptions;
using ShippingService.Models;

namespace ShippingService.Services;

public class TransportProtocolCreationService
{
    private readonly ApplicationContext _applicationContext;
    private readonly Func<decimal, decimal, decimal, decimal, IPackingService> _packingServiceFactory;
    private readonly ShippingSimulationService _shippingSimulationService;

    public TransportProtocolCreationService(ApplicationContext applicationContext,
        Func<decimal, decimal, decimal, decimal, IPackingService> packingServiceFactory,
        ShippingSimulationService shippingSimulationService)
    {
        _applicationContext = applicationContext;
        _packingServiceFactory = packingServiceFactory;
        _shippingSimulationService = shippingSimulationService;
    }

    public async Task CreateTransportProtocolAsync(TransportProtocolCreationOptions transportProtocolCreationOptions)
    {
        await _shippingSimulationService.SimulateAsync();
        var fromSeaportId = transportProtocolCreationOptions.FromSeaportId;
        var toSeaportId = transportProtocolCreationOptions.ToSeaportId;

        using var transaction = await _applicationContext.Database.BeginTransactionAsync();

        try
         {
            var fromSeaport = await _applicationContext.Seaports.FindAsync(fromSeaportId);
            if (fromSeaport is null)
                throw new Exception();

            int bestShipId;
            try
            {
                bestShipId = await TryFindBestShip(fromSeaportId, transportProtocolCreationOptions.CargoIds);
            }
            catch
            {
                throw new BestShipFindingException();
            }
            await LoadShip(transportProtocolCreationOptions, bestShipId);
            await SendShip(fromSeaportId, toSeaportId, bestShipId);

            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task SendShip(int fromSeaportId, int toSeaportId, int bestShipId)
    {
        var ship = await _applicationContext.Ships
                        .Include(ship => ship.MooredShip)
                        .FirstOrDefaultAsync(ship => ship.Id == bestShipId);
        if (ship!.MooredShip is null)
            throw new Exception();

        var toSeaport = await _applicationContext.Seaports.FindAsync(toSeaportId);
        if (toSeaport is null)
            throw new Exception();

        var shipping = new Shipping()
        {
            FromSeaportId = fromSeaportId,
            ToSeaportId = toSeaportId,
            DepartureTime = DateTime.UtcNow,
            ShipId = ship.Id,
        };

        _applicationContext.MooredShips.Remove(ship.MooredShip);
        await _applicationContext.Shippings.AddAsync(shipping);
        await _applicationContext.SaveChangesAsync();
    }

    private async Task<int> TryFindBestShip(int seaportId, List<int> cargoIds)
    {
        Ship bestShip = null!;
        var seaport = await _applicationContext.Seaports
            .Include(seaport => seaport.MooredShips)
            .ThenInclude(mooredShip => mooredShip.Ship)
            .FirstOrDefaultAsync(seaport => seaport.Id == seaportId);
        if (seaport is null)
            throw new Exception();

        foreach (var mooredShip in seaport.MooredShips)
        {
            var shipId = mooredShip.ShipId;
            if (await IsShipCanLoaded(cargoIds, shipId))
            {
                var ship = await _applicationContext.Ships.FindAsync(shipId);
                if (ship == null)
                    throw new Exception();

                if (bestShip == null)
                {
                    bestShip = ship;
                    continue;
                }

                bestShip = ship.MaxSpeedInMetersPerSecond > bestShip.MaxSpeedInMetersPerSecond ? ship : bestShip;
            }
        }

        if (bestShip == null)
            throw new Exception();

        return bestShip.Id;
    }

    private async Task<bool> IsShipCanLoaded(List<int> cargoIds, int shipId)
    {
        var ship = await _applicationContext.Ships
            .Include(ship => ship.CargoCompartment)
            .FirstOrDefaultAsync(ship => ship.Id == shipId);
        if (ship == null)
            throw new Exception();

        var isShipAlreadyContainsCargo = await _applicationContext.TransportProtocols.AnyAsync(protocol => protocol.CargoCompartment.Ship.Id == ship.Id);
        if (isShipAlreadyContainsCargo)
            return false;

        var cargoCompartment = ship.CargoCompartment;

        var cargos = await _applicationContext.Cargos.Where(cargo => cargoIds.Contains(cargo.Id)).ToListAsync();

        IPackingService packingService = _packingServiceFactory(cargoCompartment.Length, cargoCompartment.Width, cargoCompartment.Height, cargoCompartment.LoadCapacity);

        return packingService.TryPack(cargos.Cast<IItem>().ToList(), out _);
    }

    private async Task LoadShip(TransportProtocolCreationOptions transportProtocolCreationOptions, int shipId)
    {
        var ship = await _applicationContext.Ships
                        .Include(ship => ship.CargoCompartment)
                        .Include(ship => ship.MooredShip)
                        .ThenInclude(mooredShip => mooredShip.Seaport)
                        .FirstOrDefaultAsync(ship => ship.Id == shipId);
        if (ship is null || ship.MooredShip is null)
            throw new Exception();
        var seaportId = ship.MooredShip.SeaportId;
        var cargoCompartment = ship.CargoCompartment;

        var cargoIds = transportProtocolCreationOptions.CargoIds;
        var areCargosAvailable = await _applicationContext.Cargos
            .Include(cargo => cargo.Storage)
            .ThenInclude(storage => storage!.Seaport)
            .Include(cargo => cargo.TransportProtocol)
            .Where(cargo => cargoIds.Contains(cargo.Id))
            .AllAsync(cargo => cargo.Storage!.Seaport.Id == seaportId && cargo.TransportProtocol == null);

        if (!areCargosAvailable)
            throw new Exception();

        var isShipAlreadyContainsCargo = await _applicationContext.TransportProtocols.AnyAsync(protocol => protocol.CargoCompartment.Ship.Id == ship.Id);
        if (isShipAlreadyContainsCargo)
            throw new Exception();


        var cargos = await _applicationContext.Cargos
            .Include(cargo => cargo.Storage).Where(cargo => cargoIds.Contains(cargo.Id)).ToListAsync();

        IPackingService packingService = _packingServiceFactory(cargoCompartment.Length, cargoCompartment.Width, cargoCompartment.Height, cargoCompartment.LoadCapacity);

        if (!packingService.TryPack(cargos.Cast<IItem>().ToList(), out List<(int id, decimal x, decimal y, decimal z)> result))
        {
            throw new Exception();
        }

        var transportProtocols = new List<TransportProtocol>();

        foreach (var (id, x, y, z) in result)
        {
            var cargo = cargos.First(cargo => cargo.Id == id);
            cargo.Storage = null!;

            var transportProtocol = new TransportProtocol()
            {
                CargoCompartment = cargoCompartment,
                PositionX = x,
                PositionY = y,
                PositionZ = z
            };
            cargo.TransportProtocol = transportProtocol;
            
            transportProtocols.Add(transportProtocol);
        }

        await _applicationContext.TransportProtocols.AddRangeAsync(transportProtocols);
        await _applicationContext.SaveChangesAsync();
    }
}
