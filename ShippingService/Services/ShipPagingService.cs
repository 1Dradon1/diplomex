using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Extensions;
using ShippingService.Models;

namespace ShippingService.Services;

public class ShipPagingService
{
    private readonly ApplicationContext _applicationContext;
    private readonly ShippingSimulationService _shippingSimulationService;

    public ShipPagingService(ApplicationContext applicationContext,
        ShippingSimulationService shippingSimulationService)
    {
        _applicationContext = applicationContext;
        _shippingSimulationService = shippingSimulationService;
    }

    public async Task<Ship[]> GetShipsAsync(PagingOptions pagingOptions)
    {
        await _shippingSimulationService.SimulateAsync();
        return await _applicationContext.Ships.Include(ship => ship.CargoCompartment).Paginate(pagingOptions).ToArrayAsync();
    }
}