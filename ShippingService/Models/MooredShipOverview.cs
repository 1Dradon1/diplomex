using ShippingService.Entities;

namespace ShippingService.Models;

public class MooredShipOverview
{
    public MooredShip MooredShip { get; set; } = null!;
    public int CargoCount { get; set; }
}