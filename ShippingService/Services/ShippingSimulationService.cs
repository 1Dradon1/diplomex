using DecimalMath;
using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;

namespace ShippingService.Services;

public class ShippingSimulationService
{
    private readonly ApplicationContext _applicationContext;

    public ShippingSimulationService(ApplicationContext applicationContext)
    {
        _applicationContext = applicationContext;
    }

    public async Task SimulateAsync()
    {
        var shippings = await _applicationContext.Shippings
            .Include(shipping => shipping.FromSeaport)
            .Include(shipping => shipping.ToSeaport)
            .Include(shipping => shipping.Ship)
            .ToListAsync();

        foreach (var shipping in shippings)
            if (IsArrived(shipping))
                await Moor(shipping);
    }

    private async Task Moor(Shipping shipping)
    {
        var ship = shipping.Ship;
        var mooredShip = new MooredShip()
        {
            SeaportId = shipping.ToSeaportId,
            ShipId = ship.Id
        };

        ship.MooredShip = mooredShip;
        ship.Shipping = null!;
        _applicationContext.Remove(shipping);
        await _applicationContext.SaveChangesAsync();
    }

    private bool IsArrived(Shipping shipping)
    {
        var shippingTimeInIntegerSeconds = (int)(DateTime.UtcNow - shipping.DepartureTime).TotalSeconds;
        var speedInMetersPerSecond = shipping.Ship.MaxSpeedInMetersPerSecond;

        var fromSeaport = shipping.FromSeaport;
        var toSeaport = shipping.ToSeaport;

        var fromX = fromSeaport.PositionX;
        var fromY = fromSeaport.PositionY;

        var toX = toSeaport.PositionX;
        var toY = toSeaport.PositionY;

        var totalDistance = GetDistance(fromX, fromY, toX, toY);

        var traveledDistance = speedInMetersPerSecond * shippingTimeInIntegerSeconds;

        return traveledDistance >= totalDistance;
    }

    private decimal GetDistance(decimal firstPointX, decimal firstPointY, decimal secondPointX, decimal secondPointY)
    {
        var point1 = new Location() { Latitude = DecimalToDegrees(firstPointX + 0.00000000000000001m), Longitude = DecimalToDegrees(firstPointY + 0.00000000000000001m) };
        var point2 = new Location() { Latitude = DecimalToDegrees(secondPointX + 0.00000000000000001m), Longitude = DecimalToDegrees(secondPointY + 0.00000000000000001m) };

        var d1 = point1.Latitude * (DecimalEx.Pi / 180.0m);
        var num1 = point1.Longitude * (DecimalEx.Pi / 180.0m);
        var d2 = point2.Latitude * (DecimalEx.Pi / 180.0m);
        var num2 = point2.Longitude * (DecimalEx.Pi / 180.0m) - num1;
        var d3 = DecimalEx.Pow(DecimalEx.Sin((d2 - d1) / 2.0m), 2.0m) +
                 DecimalEx.Cos(d1) * DecimalEx.Cos(d2) * DecimalEx.Pow(DecimalEx.Sin(num2 / 2.0m), 2.0m);

        var distance = 6371800.0m * (2.0m * DecimalEx.ATan2(DecimalEx.Sqrt(d3), DecimalEx.Sqrt(1.0m - d3)));
        return distance;
    }

    private decimal DecimalToDegrees(decimal value)
    {
        var valueString = value.ToString();
        var parts = valueString.Split('.').ToArray();
        if (parts.Length == 1)
            return value;
        var secondPart = parts[1];

        var degrees = decimal.Parse(parts[0]);
        var minutes = decimal.Parse(secondPart.Substring(0, 2));
        var seconds = decimal.Parse((secondPart.Substring(2, 2)));

        return degrees + (minutes / 60) + (seconds / 3600);
    }

    public class Location
    {
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
    }
}