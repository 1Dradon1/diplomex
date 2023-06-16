using ShippingService.Models;

namespace ShippingService.Services;

public interface IPackingService
{
    public bool TryPack(List<IItem> packingContainers, out List<(int id, decimal x, decimal y, decimal z)> result);
}
