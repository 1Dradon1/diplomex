using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShippingService.Entities;

public class Ship
{
    public int Id { get; set; } 
    public decimal MaxSpeedInMetersPerSecond { get; set; }
    public string Name { get; set; } = null!;
    [JsonIgnore]
    [ForeignKey(nameof(CargoCompartment))]
    public int CargoCompartmentId { get; set; }

    public CargoCompartment CargoCompartment { get; set; } = null!;

    [JsonIgnore]
    public MooredShip MooredShip { get; set; } = null!;
    [JsonIgnore]
    public Shipping Shipping { get; set; } = null!;
}