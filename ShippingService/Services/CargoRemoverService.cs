using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Models;

namespace ShippingService.Services;

public class CargoRemoverService
{
    private readonly ApplicationContext _applicationContext;

    public CargoRemoverService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task RemoveCargosAsync(CargoRemovingOptions cargoRemovingOptions)
    {
        var seaportId = cargoRemovingOptions.SeaportId;
        var cargoIds = cargoRemovingOptions.CargoIds;

        var cargos = await _applicationContext.Cargos
            .Include(cargo => cargo.TransportProtocol)
            .Include(cargo => cargo.Storage)
            .ThenInclude(storage => storage!.Seaport)
            .Where(cargo => cargoIds.Contains(cargo.Id))
            .ToArrayAsync();

        var isPossible = cargos.All(cargo => cargo.TransportProtocol == null && cargo.Storage != null && cargo.Storage.Seaport.Id == seaportId);
        if (!isPossible)
            throw new Exception("Not possible to remove cargos");

        _applicationContext.Cargos.RemoveRange(cargos);
        await _applicationContext.SaveChangesAsync();
    }
}
