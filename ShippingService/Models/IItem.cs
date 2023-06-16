namespace ShippingService.Models;

public interface IItem
{
    public int Id { get; }
    public decimal Length { get; }
    public decimal Width { get; }
    public decimal Height { get; }
    public decimal Weight { get; }
}