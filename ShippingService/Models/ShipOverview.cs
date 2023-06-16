namespace ShippingService.Models;

public class ShipOverview
{
    public int ShipId { get; set; }
    public bool IsMoored { get; set; }
    public int? SeaportId { get; set; }
    public int? ShippingId { get; set; }
}