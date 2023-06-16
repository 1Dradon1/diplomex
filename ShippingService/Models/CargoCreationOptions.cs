namespace ShippingService.Models;

public sealed class CargoCreationOptions
{
    public string Title { get; set; } = null!;
    public string Description { get; set; } = null!;
    public decimal Weight { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public int StorageId { get; set; }
}