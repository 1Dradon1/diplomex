using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;

namespace ShippingService.Services;

public class SeaportRemoverService
{
    private readonly ApplicationContext _applicationContext;

    public SeaportRemoverService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task RemoveSeaportAsync(int id)
    {
        using var transaction = await _applicationContext.Database.BeginTransactionAsync();

        try
        {
            var seaport = await _applicationContext.Seaports
            .Include(seaport => seaport.Storage).ThenInclude(storage => storage.Cargos)
            .Include(seaport => seaport.MooredShips)
            .Include(seaport => seaport.FromShippings)
            .Include(seaport => seaport.ToShippings)
            .FirstOrDefaultAsync(seaport => seaport.Id == id)
            ?? throw new ArgumentException("Seaport is not exist");

            if (!IsStorageEmpty(seaport.Storage) || HasMooredShips(seaport) || HasRelatedShippings(seaport))
                throw new InvalidOperationException();

            _applicationContext.Seaports.Remove(seaport);
            _applicationContext.Storages.Remove(seaport.Storage);
            await _applicationContext.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private bool HasMooredShips(Seaport seaport)
        => seaport.MooredShips.Any();

    private bool IsStorageEmpty(Storage storage)
        => !storage.Cargos.Any();

    private bool HasRelatedShippings(Seaport seaport)
        => seaport.FromShippings.Any() || seaport.ToShippings.Any();
}