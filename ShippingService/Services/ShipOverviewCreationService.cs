using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Models;

namespace ShippingService.Services;

public class ShipOverviewCreationService
{
    private readonly ApplicationContext _applicationContext;
    private readonly ShippingSimulationService _shippingSimulationService;

    public ShipOverviewCreationService(ApplicationContext applicationContext, ShippingSimulationService shippingSimulationService)
    {
        _applicationContext = applicationContext;
        _shippingSimulationService = shippingSimulationService;
    }

    public async Task<ShipOverview[]> GetShipOverviewsAsync()
    {
        await _shippingSimulationService.SimulateAsync();
        var ships = await _applicationContext.Ships
            .Include(ship => ship.Shipping)
            .Include(ship => ship.MooredShip)
            .ThenInclude(mooredShip => mooredShip.Seaport)
            .ToListAsync();

        return ships.Select(CreateOverview).ToArray();
    }

    private ShipOverview CreateOverview(Ship ship)
    {
        var shipOverview = new ShipOverview()
        {
            ShipId = ship.Id,
            IsMoored = ship.MooredShip != null,
            SeaportId = ship?.MooredShip?.SeaportId,
            ShippingId = ship?.Shipping?.Id
        };

        return shipOverview;
    }
}
