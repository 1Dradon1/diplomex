using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Extensions;
using ShippingService.Models;

namespace ShippingService.Services;

public class ShippingPagingService
{
    private readonly ApplicationContext _applicationContext;

    public ShippingPagingService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Shipping[]> GetShippingsAsync(PagingOptions pagingOptions)
    {
        return await _applicationContext.Shippings
        .Include(shipping => shipping.Ship)
        .Include(cargo => cargo.Ship).ThenInclude(ship => ship.CargoCompartment)
        .Include(shipping => shipping.FromSeaport)
        .Include(shipping => shipping.ToSeaport)
        .Paginate(pagingOptions)
        .ToArrayAsync();
    }
}