using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace ShippingService.Entities;

public class Seaport
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public decimal PositionX { get; set; }
    public decimal PositionY { get; set; }
    [ForeignKey(nameof(Storage))]
    public int StorageId { get; set; }

    [JsonIgnore]
    public Storage Storage { get; set; } = null!;
    [JsonIgnore]
    public List<Shipping> FromShippings { get; set; } = null!;
    [JsonIgnore]
    public List<Shipping> ToShippings { get; set; } = null!;
    [JsonIgnore]
    public List<MooredShip> MooredShips { get; set; } = null!;
}