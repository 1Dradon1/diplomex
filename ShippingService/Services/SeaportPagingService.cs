using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Extensions;
using ShippingService.Models;

namespace ShippingService.Services;

public class SeaportPagingService
{
    private readonly ApplicationContext _applicationContext;

    public SeaportPagingService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public Task<Seaport[]> GetSeaportsAsync()
        => _applicationContext.Seaports.ToArrayAsync();

    public async Task<Seaport?> GetSeaportAsync(int seaportId)
        => await _applicationContext.Seaports
        .Include(seaport => seaport.Storage)
        .FirstOrDefaultAsync(seaport => seaport.Id == seaportId);
}