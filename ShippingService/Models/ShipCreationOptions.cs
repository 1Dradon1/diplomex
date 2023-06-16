namespace ShippingService.Models;

public sealed class ShipCreationOptions
{
    public CargoCompartmentCreationOptions CargoCompartmentOptions { get; set; } = null!;
    public decimal MaxSpeedInMetersPerSecond { get; set; }
    public string Name { get; set; } = null!;
    public int SeaportId { get; set; }
}