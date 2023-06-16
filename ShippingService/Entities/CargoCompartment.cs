using System.Text.Json.Serialization;

namespace ShippingService.Entities;

public class CargoCompartment
{
    public int Id { get; set; }
    public decimal Length { get; set; }
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal LoadCapacity { get; set; }

    [JsonIgnore]
    public Ship Ship { get; set; } = null!;
    [JsonIgnore]
    public List<TransportProtocol> TransportProtocols { get; set; } = null!;
}