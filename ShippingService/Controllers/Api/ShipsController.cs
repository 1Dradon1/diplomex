using Microsoft.AspNetCore.Mvc;
using ShippingService.Entities;
using ShippingService.Exceptions;
using ShippingService.Models;
using ShippingService.Services;

namespace ShippingService.Controllers.Api;

[ApiController]
[Route("api/{controller}")]
public class ShipsController : ControllerBase
{
    private readonly ShipCreationService _shipCreationService;
    private readonly ShipOverviewCreationService _shipOverviewCreationService;
    private readonly MooredShipOverviewCreationService _mooredShipOverviewCreationService;
    private readonly MooredShipRemoverService _mooredShipRemoverService;

    public ShipsController(
        ShipCreationService shipCreationService,
        ShipOverviewCreationService shipOverviewCreationService,
        MooredShipOverviewCreationService mooredShipOverviewCreationService,
        MooredShipRemoverService mooredShipRemoverService)
    {
        _shipCreationService = shipCreationService;
        _shipOverviewCreationService = shipOverviewCreationService;
        _mooredShipOverviewCreationService = mooredShipOverviewCreationService;
        _mooredShipRemoverService = mooredShipRemoverService;
    }

    [Route("overview")]
    [HttpPost]
    public async Task<ShipOverview[]> GetShipOverviewsAsync()
    => await _shipOverviewCreationService.GetShipOverviewsAsync();

    [Route("add")]
    [HttpPost]
    public async Task<ContentResult> CreateShipAsync([FromBody] ShipCreationOptions shipCreationOptions)
    {
        try
        {
            await _shipCreationService.CreateShipAsync(shipCreationOptions);
        }
        catch (Exception)
        {
            return Content("InternalException");
        }
        return Content("OK");
    }

    [Route("moored")]
    [HttpPost]
    public async Task<MooredShipOverview[]> GetMooredShipOverviewsAsync([FromQuery] int seaportId)
        => await _mooredShipOverviewCreationService.GetMooredShipOverviewsAsync(seaportId);

    [Route("remove")]
    [HttpPost]
    public async Task<ContentResult> RemoveMooredShipAsync([FromQuery] int shipId, [FromQuery] int seaportId)
    {
        try
        {
            await _mooredShipRemoverService.RemoveMooredShipAsync(shipId, seaportId);
        }
        catch(Exception)
        {
            return Content("InternalException");
        }
        return Content("OK");
    }
}
