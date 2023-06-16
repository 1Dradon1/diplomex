namespace ShippingService.Models;

public class TransportProtocolCreationOptions
{
    public List<int> CargoIds { get; set; } = null!;
    public int FromSeaportId { get; set; }
    public int ToSeaportId { get; set; }
}