using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Models;

namespace ShippingService.Services;

public class MooredShipOverviewCreationService
{
    private readonly ApplicationContext _applicationContext;
    private readonly ShippingSimulationService _shippingSimulationService;

    public MooredShipOverviewCreationService(ApplicationContext applicationContext,
        ShippingSimulationService shippingSimulationService)
    {
        _applicationContext = applicationContext;
        _shippingSimulationService = shippingSimulationService;
    }

    public async Task<MooredShipOverview[]> GetMooredShipOverviewsAsync(int seaportId)
    {
        await _shippingSimulationService.SimulateAsync();
        var mooredShips = await _applicationContext.MooredShips
            .Include(mooredShip => mooredShip.Ship)
            .ThenInclude(ship => ship.CargoCompartment)
            .Include(mooredShip => mooredShip.Seaport)
            .Where(mooredShip => mooredShip.SeaportId == seaportId)
            .ToArrayAsync();


        var result = new MooredShipOverview[mooredShips.Length];
        for (var i = 0; i < mooredShips.Length; i++)
        {
            var mooredShip = mooredShips[i];

            var mooredShipOverview = new MooredShipOverview()
            {
                MooredShip = mooredShip,
                CargoCount = await CountCargo(mooredShip.Ship.CargoCompartmentId)
            };
            result[i] = mooredShipOverview;
        }

        return result;
    }

    private async Task<int> CountCargo(int cargoCompartmentId)
    {
        return await _applicationContext.TransportProtocols
            .Include(protocol => protocol.CargoCompartment)
            .Include(protocol => protocol.Cargo)
            .Where(protocol => protocol.CargoCompartmentId == cargoCompartmentId && protocol.Cargo != null)
            .CountAsync();
    }
}
