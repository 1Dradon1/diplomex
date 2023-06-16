using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Extensions;
using ShippingService.Models;

namespace ShippingService.Services;

public class CargoPagingService
{
    private readonly ApplicationContext _applicationContext;

    public CargoPagingService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task<Cargo[]> GetCargosAsync(int seaportId, PagingOptions pagingOptions)
    {
        return await _applicationContext.Cargos
        .Include(cargo => cargo.Storage)
        .ThenInclude(storage => storage!.Seaport)
        .Include(cargo => cargo.TransportProtocol)
        .Where(cargo => cargo.Storage != null && cargo.Storage.Seaport.Id == seaportId)
        .Paginate(pagingOptions)
        .ToArrayAsync();
    }

    public async Task<int> GetTotalPagesCount(int seaportId, int itemsCountPerPage)
    {
        if (itemsCountPerPage <= 0)
            return 0;

        return (int)Math.Ceiling((await _applicationContext.Cargos
        .Include(cargo => cargo.Storage)
        .ThenInclude(storage => storage!.Seaport)
        .CountAsync(cargo => cargo.Storage != null && cargo.Storage.Seaport.Id == seaportId)) / (double)itemsCountPerPage);
    }
}
