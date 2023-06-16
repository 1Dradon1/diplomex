using Microsoft.AspNetCore.Mvc;
using ShippingService.Entities;
using ShippingService.Exceptions;
using ShippingService.Models;
using ShippingService.Services;

namespace ShippingService.Controllers.Api;

[ApiController]
[Route("api/{controller}")]
public class CargosController : ControllerBase
{
    private readonly CargoCreationService _cargoCreationService;
    private readonly TransportProtocolCreationService _transportProtocolCreationService;
    private readonly CargoUnloadingService _cargoUnloadingService;
    private readonly ShippingPagingService _shippingPagingService;
    private readonly CargoPagingService _cargoPagingService;
    private readonly CargoRemoverService _cargoRemoverService;

    public CargosController(CargoCreationService cargoCreationService,
        TransportProtocolCreationService transportProtocolCreationService,
        CargoUnloadingService cargoUnloadingService,
        ShippingPagingService shippingPagingService,
        CargoPagingService cargoPagingService,
        CargoRemoverService cargoRemoverService)
    {
        _cargoCreationService = cargoCreationService;
        _transportProtocolCreationService = transportProtocolCreationService;
        _cargoUnloadingService = cargoUnloadingService;
        _shippingPagingService = shippingPagingService;
        _cargoPagingService = cargoPagingService;
        _cargoRemoverService = cargoRemoverService;
    }

    [Route("add")]
    [HttpPost]
    public async Task<ContentResult> AddCargoToStorage([FromBody] CargoCreationOptions cargoCreationOptions)
    {
        try
        {
            await _cargoCreationService.CreateCargoAsync(cargoCreationOptions);
        }
        catch
        {
            return Content("InternalException");
        }
        return Content("OK");
    }

    [HttpPost]
    [Route("{seaportId:int}")]
    public async Task<Cargo[]> GetCargosAsync([FromRoute] int seaportId, [FromBody] PagingOptions pagingOptions)
        => await _cargoPagingService.GetCargosAsync(seaportId, pagingOptions);

    [Route("shippings")]
    [HttpPost]
    public async Task<Shipping[]> GetShippings(PagingOptions pagingOptions)
        => await _shippingPagingService.GetShippingsAsync(pagingOptions);

    [Route("send")]
    [HttpPost]
    public async Task<ContentResult> LoadAndSendShipAsync([FromBody] TransportProtocolCreationOptions transportProtocolCreationOptions)
    {
        try
        {
            await _transportProtocolCreationService.CreateTransportProtocolAsync(transportProtocolCreationOptions);
        }
        catch (BestShipFindingException)
        {
            return Content("BestShipFindingException");
        }
        catch (Exception)
        {
            return Content("InternalException");
        }
        return Content("OK");
    }

    [Route("unload")]
    [HttpPost]
    public async Task<ContentResult> UnloadCargoToStorageAsync([FromBody] CargoUnloadingOptions cargoUnloadingOptions)
    {
        try
        {
            await _cargoUnloadingService.UnloadCargoAsync(cargoUnloadingOptions);
        }
        catch (Exception)
        {
            return Content("InternalException");
        }
        return Content("OK");
    }

    [Route("storage/{seaportId:int}")]
    [HttpPost]
    public async Task<int> GetTotalPagesCount([FromQuery] int itemsCountPerPage, [FromRoute] int seaportId)
        => await _cargoPagingService.GetTotalPagesCount(seaportId, itemsCountPerPage);

    [HttpPost]
    public async Task<ContentResult> RemoveCargosAsync([FromBody] CargoRemovingOptions cargoRemovingOptions)
    {
        try
        {
            await _cargoRemoverService.RemoveCargosAsync(cargoRemovingOptions);
        }
        catch
        {
            return Content("InternalException");
        }
        return Content("OK");
    }
}