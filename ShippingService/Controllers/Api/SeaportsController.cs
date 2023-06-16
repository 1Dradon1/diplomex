using Microsoft.AspNetCore.Mvc;
using ShippingService.Entities;
using ShippingService.Exceptions;
using ShippingService.Models;
using ShippingService.Services;

namespace ShippingService.Controllers.Api;

[ApiController]
[Route("api/{controller}")]
public class SeaportsController : ControllerBase
{
    private readonly SeaportPagingService _seaportPagingService;
    private readonly SeaportCreationService _seaportCreationService;
    private readonly SeaportRemoverService _seaportRemoverService;

    public SeaportsController(
        SeaportPagingService seaportPagingService,
        SeaportCreationService seaportCreationService,
        SeaportRemoverService seaportRemoverService)
    {
        _seaportPagingService = seaportPagingService;
        _seaportCreationService = seaportCreationService;
        _seaportRemoverService = seaportRemoverService;
    }

    [HttpPost]
    public async Task<Seaport[]> GetSeaports()
        => await _seaportPagingService.GetSeaportsAsync();

    [HttpPost]
    [Route("{seaportId:int}")]
    public async Task<Seaport?> GetSeaport([FromRoute] int seaportId)
        => await _seaportPagingService.GetSeaportAsync(seaportId);

    [HttpPost]
    [Route("add")]
    public async Task<ContentResult> CreateSeaport(SeaportCreationOptions seaportCreationOptions)
    {
        try
        {
            await _seaportCreationService.CreateSeaportAsync(seaportCreationOptions);
        }
        catch(PositionAlreadyOccupiedException)
        {
            return Content("PositionAlreadyOccupiedException");
        }
        catch(Exception)
        {
            return Content("InternalException");
        }
        return Content("OK");
    }

    [HttpPost]
    [Route("remove")]
    public async Task<ContentResult> RemoveSeaport([FromQuery] int id)
    {
        try
        {
            await _seaportRemoverService.RemoveSeaportAsync(id);
        }
        catch
        {
            return Content("InternalException");
        }
        return Content("OK");
    }
}