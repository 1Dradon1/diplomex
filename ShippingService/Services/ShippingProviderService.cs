using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;

namespace ShippingService.Services;

public class ShippingProviderService
{
    private readonly ApplicationContext _applicationContext;

    public ShippingProviderService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Shipping[]> GetShippingsAsync()
    {
        return await _applicationContext.Shippings
            .Include(shipping => shipping.Ship)
            .Include(shipping => shipping.FromSeaport)
            .Include(shipping => shipping.ToSeaport)
            .ToArrayAsync();
    }
}