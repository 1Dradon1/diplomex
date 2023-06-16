using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingService.Entities;

public class Shipping
{
    public int Id { get; set; }
    public int ShipId { get; set; }
    [ForeignKey(nameof(FromSeaport))]
    public int FromSeaportId { get; set; }
    public DateTime DepartureTime { get; set; }
    [ForeignKey(nameof(ToSeaport))]
    public int ToSeaportId { get; set; }

    public Seaport FromSeaport { get; set; } = null!;
    public Seaport ToSeaport { get; set; } = null!;
    public Ship Ship { get; set; } = null!;
}