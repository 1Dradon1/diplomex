﻿using Microsoft.AspNetCore.Mvc;
using ShippingService.ActionResults;

namespace ShippingService.Controllers;

[ApiController]
[Route("{controller}")]
public class SeaportManagersController : Controller
{
    private readonly Func<string, HtmlResult> _htmlResultFactory;

    public SeaportManagersController(Func<string, HtmlResult> htmlResultFactory)
    {
        _htmlResultFactory = htmlResultFactory;
    }

    [HttpGet]
    public IActionResult Get()
        => _htmlResultFactory(@"./wwwroot/html/seaportManager.html");
}