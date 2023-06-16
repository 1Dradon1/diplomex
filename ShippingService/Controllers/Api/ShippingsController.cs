using Microsoft.AspNetCore.Mvc;
using ShippingService.Entities;
using ShippingService.Services;

namespace ShippingService.Controllers.Api;

[ApiController]
[Route("api/{controller}")]
public class ShippingsController : ControllerBase
{
    private readonly ShippingProviderService _shippingProviderService;

    public ShippingsController(ShippingProviderService shippingProviderService)
    {
        _shippingProviderService = shippingProviderService;
    }

    [HttpPost]
    public Task<Shipping[]> GetShippingsAsync()
        => _shippingProviderService.GetShippingsAsync();
}
