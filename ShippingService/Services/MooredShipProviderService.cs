using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;

namespace ShippingService.Services;

public class MooredShipProviderService
{
    private readonly ApplicationContext _applicationContext;

    public MooredShipProviderService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public Task<MooredShip[]> GetMooredShipsAsync(int seaportId)
        => _applicationContext.MooredShips
        .Include(mooredShip => mooredShip.Ship)
        .Include(mooredShip => mooredShip.Seaport)
        .Where(mooredShip => mooredShip.SeaportId == seaportId)
        .ToArrayAsync();
}