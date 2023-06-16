using Microsoft.EntityFrameworkCore;
using ShippingService.Data;

namespace ShippingService.Services;

public class MooredShipRemoverService
{
    private readonly ApplicationContext _applicationContext;

    public MooredShipRemoverService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task RemoveMooredShipAsync(int shipId, int seaportId)
    {
        var ship = await _applicationContext.Ships
            .Include(ship => ship.CargoCompartment)
            .Include(ship => ship.MooredShip)
            .ThenInclude(mooredShip => mooredShip.Seaport)
            .FirstOrDefaultAsync(ship => ship.Id == shipId);

        if (ship?.MooredShip?.SeaportId != seaportId)
            throw new Exception("Ship is not moored or moored at another seaport");

        var cargoCompartmentId = ship.CargoCompartmentId;
        var hasCargos = await _applicationContext.TransportProtocols
            .Include(protocol => protocol.CargoCompartment)
            .Include(protocol => protocol.Cargo)
            .Where(protocol => protocol.CargoCompartmentId == cargoCompartmentId && protocol.Cargo != null)
            .AnyAsync();

        if (hasCargos)
            throw new Exception("Ship with cargo can't be removed");

        _applicationContext.Ships.Remove(ship);
        await _applicationContext.SaveChangesAsync();
    }
}