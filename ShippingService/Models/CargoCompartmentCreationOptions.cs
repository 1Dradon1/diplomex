namespace ShippingService.Models;

public sealed class CargoCompartmentCreationOptions
{
    public decimal Height { get; set; }
    public decimal Width { get; set; }
    public decimal Length { get; set; }
    public decimal LoadCapacity { get; set; }
}