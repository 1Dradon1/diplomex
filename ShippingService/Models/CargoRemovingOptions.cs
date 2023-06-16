namespace ShippingService.Models;

public class CargoRemovingOptions
{
    public int SeaportId { get; set; }
    public List<int> CargoIds { get; set; } = null!;
}