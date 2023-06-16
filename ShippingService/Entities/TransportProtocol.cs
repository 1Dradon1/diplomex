using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShippingService.Entities;

public class TransportProtocol
{
    public int Id { get; set; }
    [ForeignKey(nameof(CargoCompartment))]
    public int CargoCompartmentId { get; set; }
    public decimal PositionX { get; set; }
    public decimal PositionY { get; set; }
    public decimal PositionZ { get; set; }

    public CargoCompartment CargoCompartment { get; set; } = null!;
    [JsonIgnore]
    public Cargo Cargo { get; set; } = null!;
}