using System.Text.Json.Serialization;

namespace ShippingService.Entities;

public class Storage
{
    public int Id { get; set; }
    public Seaport Seaport { get; set; } = null!;

    [JsonIgnore]
    public List<Cargo> Cargos { get; set; } = null!;
}