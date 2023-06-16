using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Exceptions;
using ShippingService.Models;

namespace ShippingService.Services;

public sealed class SeaportCreationService
{
    private readonly ApplicationContext _applicationContext;
    private readonly IMapper _mapper;

    public SeaportCreationService(ApplicationContext applicationContext,
        IMapper mapper)
    {
        _applicationContext = applicationContext;
        _mapper = mapper;
    }

    public async Task CreateSeaportAsync(
        SeaportCreationOptions seaportCreationOptions)
    {
        var positionX = seaportCreationOptions.PositionX;
        var positionY = seaportCreationOptions.PositionY;

        if (await IsSeaportPlaceAlreadyOccupiedAsync(positionX, positionY))
            throw new PositionAlreadyOccupiedException();

        var storage = new Storage();
        var seaport = _mapper.Map<Seaport>(seaportCreationOptions);
        seaport.Storage = storage;

        await _applicationContext.AddRangeAsync(new object[] { storage, seaport });
        await _applicationContext.SaveChangesAsync();
    }

    private async Task<bool> IsSeaportPlaceAlreadyOccupiedAsync(decimal positionX, decimal positionY)
    {
        var seaport = await _applicationContext.Seaports
            .FirstOrDefaultAsync(sp => sp.PositionY == positionY && sp.PositionX == positionX);

        return seaport is not null;
    }
}