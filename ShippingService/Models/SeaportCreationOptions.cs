namespace ShippingService.Models;

public class SeaportCreationOptions
{
    public string Name { get; set; } = null!;
    public decimal PositionX { get; set; }
    public decimal PositionY { get; set; }
}