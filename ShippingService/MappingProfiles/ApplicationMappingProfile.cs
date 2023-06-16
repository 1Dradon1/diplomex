using AutoMapper;
using ShippingService.Entities;
using ShippingService.Models;

namespace ShippingService.MappingProfiles;

public class ApplicationMappingProfile : Profile
{
    public ApplicationMappingProfile()
    {
        CreateMap<CargoCreationOptions, Cargo>();
        CreateMap<SeaportCreationOptions, Seaport>();
        CreateMap<CargoCompartmentCreationOptions, CargoCompartment>();
        CreateMap<ShipCreationOptions, Ship>();
        CreateMap<ShipCreationOptions, MooredShip>();
    }
}