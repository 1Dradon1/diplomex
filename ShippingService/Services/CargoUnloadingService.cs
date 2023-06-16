using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Models;

namespace ShippingService.Services;

public class CargoUnloadingService
{
    private readonly ApplicationContext _applicationContext;
    private readonly ShippingSimulationService _shippingSimulationService;

    public CargoUnloadingService(ApplicationContext applicationContext, ShippingSimulationService shippingSimulationService)
    {
        _applicationContext = applicationContext;
        _shippingSimulationService = shippingSimulationService;
    }

    public async Task UnloadCargoAsync(CargoUnloadingOptions cargoCreationOptions)
    {
        await _shippingSimulationService.SimulateAsync();
        using var transaction = await _applicationContext.Database.BeginTransactionAsync();
        try
        {
            var shipId = cargoCreationOptions.ShipId;
            var seaportId = cargoCreationOptions.SeaportId;
            var ship = await _applicationContext.Ships
                .Include(ship => ship.MooredShip)
                .ThenInclude(mooredShip => mooredShip.Seaport)
                .ThenInclude(seaport => seaport.Storage)
                .Include(ship => ship.CargoCompartment)
                .FirstOrDefaultAsync(ship => ship.Id == shipId);

            if (ship?.MooredShip?.SeaportId != seaportId || ship?.MooredShip?.Seaport?.Storage == null)
                throw new Exception();

            var cargos = await _applicationContext.Cargos
                .Include(cargo => cargo.TransportProtocol)
                .ThenInclude(transportProtocol => transportProtocol!.CargoCompartment)
                .Include(cargo => cargo.Storage)
                .Where(cargo => cargo.TransportProtocol != null && cargo.TransportProtocol.CargoCompartmentId == ship.CargoCompartmentId).ToListAsync();

            if (!cargos.Any())
                return;

            foreach (var cargo in cargos)
            {
                _applicationContext.TransportProtocols.Remove(cargo!.TransportProtocol!);
                cargo.StorageId = ship.MooredShip.Seaport.StorageId;
            }

            await _applicationContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
