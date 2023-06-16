using AutoMapper;
using ShippingService.Data;
using ShippingService.Entities;
using ShippingService.Models;

namespace ShippingService.Services;

public class CargoCreationService
{
    private readonly ApplicationContext _applicationContext;
    private readonly IMapper _mapper;

    public CargoCreationService(ApplicationContext applicationContext, IMapper mapper)
    {
        _applicationContext = applicationContext;
        _mapper = mapper;
    }

    public async Task CreateCargoAsync(CargoCreationOptions cargoCreationOptions)
    {
        using var transaction = await _applicationContext.Database.BeginTransactionAsync();

        try
        {
            var cargo = _mapper.Map<Cargo>(cargoCreationOptions);

            await _applicationContext.Cargos.AddAsync(cargo);
            await _applicationContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch(Exception)
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}