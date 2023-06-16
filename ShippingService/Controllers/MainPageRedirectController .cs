using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace OnlineStore.Controllers.Redirect;

[Route("")]
[ApiController]
public class MainPageRedirectController : Controller
{
    private const string MainPageRouteTemplate = "/seaports";

    [HttpGet]
    public IActionResult IndexGet()
    {
        return Redirect(MainPageRouteTemplate);
    }
}