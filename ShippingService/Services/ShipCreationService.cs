using AutoMapper;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Models;

namespace ShippingService.Services;

public sealed class ShipCreationService
{
    private readonly ApplicationContext _applicationContext;
    private readonly IMapper _mapper;

    public ShipCreationService(ApplicationContext applicationContext,
        IMapper mapper)
    {
        _applicationContext = applicationContext;
        _mapper = mapper;
    }

    public async Task<int> CreateShipAsync(ShipCreationOptions shipCreationOptions)
    {
        _ = await _applicationContext.Seaports.FindAsync(shipCreationOptions.SeaportId)
            ?? throw new ArgumentException($"Seaport with id {shipCreationOptions.SeaportId} is not exist");

        var cargoCompartmentCreationOptions = shipCreationOptions.CargoCompartmentOptions;

        var cargoCompartment = _mapper.Map<CargoCompartment>(cargoCompartmentCreationOptions);
        var ship = CreateShip(shipCreationOptions, cargoCompartment);
        var mooredShip = CreateMooredShip(shipCreationOptions, ship);

        await _applicationContext.AddRangeAsync(new object[] { cargoCompartment, ship, mooredShip });
        await _applicationContext.SaveChangesAsync();

        return ship.Id;
    }

    private Ship CreateShip(ShipCreationOptions shipCreationOptions, CargoCompartment cargoCompartment)
    {
        var ship = _mapper.Map<Ship>(shipCreationOptions);
        ship.CargoCompartment = cargoCompartment;
        return ship;
    }

    private MooredShip CreateMooredShip(ShipCreationOptions shipCreationOptions, Ship ship)
    {
        var mooredShip = _mapper.Map<MooredShip>(shipCreationOptions);
        mooredShip.Ship = ship;
        return mooredShip;
    }
}
