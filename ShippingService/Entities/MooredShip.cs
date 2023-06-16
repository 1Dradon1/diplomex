using System.ComponentModel.DataAnnotations.Schema;

namespace ShippingService.Entities;

public class MooredShip
{
    public int Id { get; set; }
    public int SeaportId { get; set; }
    [ForeignKey(nameof(Ship))]
    public int ShipId { get; set; }

    public Seaport Seaport { get; set; } = null!;
    public Ship Ship { get; set; } = null!;
}