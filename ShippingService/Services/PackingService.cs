using CromulentBisgetti.ContainerPacking.Entities;
using ShippingService.Models;

namespace ShippingService.Services;

public sealed class PackingService : IPackingService
{
    private readonly Container _container;
    private readonly decimal _loadCapacity;
    private decimal _currentWeight;

    public PackingService(decimal length, decimal width, decimal height, decimal loadCapacity)
    {
        _container = new Container(1, length, width, height);
        _loadCapacity = loadCapacity;
    }

    public bool TryPack(List<IItem> itemsToPack, out List<(int id, decimal x, decimal y, decimal z)> result)
    {
        if (itemsToPack.Count == 0)
        {
            result = null!;
            return false;
        }
        var containers = new List<Container>() { _container };
        var items = itemsToPack.Select(item =>
        {
            _currentWeight += item.Weight;
            return new Item(item.Id, item.Length, item.Width, item.Height, 1);
        }).ToList();
        if (_currentWeight > _loadCapacity)
        {
            result = null!;
            return false;
        }
        var algorithmIds = new List<int>() { (int)CromulentBisgetti.ContainerPacking.Algorithms.AlgorithmType.EB_AFIT };
        var packingResult = CromulentBisgetti.ContainerPacking.PackingService.Pack(containers, items, algorithmIds).First();

        var algorithmResult = packingResult.AlgorithmPackingResults.First();
        var isCompleted = algorithmResult.IsCompletePack;
        if (!isCompleted)
        {
            result = null!;
            return false;
        }

        result = algorithmResult.PackedItems.Select(item => (item.ID, item.CoordX, item.CoordY, item.CoordZ)).ToList();
        return true;
    }
}
