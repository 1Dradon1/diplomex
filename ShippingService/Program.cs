using Microsoft.Extensions.DependencyInjection;
using ShippingService.ActionResults;
using ShippingService.Controllers.Api;
using ShippingService.Data;
using ShippingService.MappingProfiles;
using ShippingService.Services;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ShippingService;

internal static class Program
{
    private static void Main()
    {
        var builder = WebApplication.CreateBuilder();
        builder.Services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        }));
        builder.Services.AddDbContext<ApplicationContext>();
        builder.Services.AddControllers()
            .AddJsonOptions(options =>
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)));
        builder.Services.AddAutoMapper(typeof(ApplicationMappingProfile));

        builder.Services.AddTransient<CargoPagingService>();
        builder.Services.AddTransient<ShippingPagingService>();
        builder.Services.AddTransient<ShippingSimulationService>();
        builder.Services.AddTransient<ShipOverviewCreationService>();
        builder.Services.AddTransient<CargoCreationService>();
        builder.Services.AddTransient<CargoUnloadingService>();
        builder.Services.AddTransient<TransportProtocolCreationService>();
        builder.Services.AddTransient<SeaportCreationService>();
        builder.Services.AddTransient<SeaportPagingService>();
        builder.Services.AddTransient<SeaportRemoverService>();
        builder.Services.AddTransient<ShipCreationService>();
        builder.Services.AddTransient<ShipPagingService>();
        builder.Services.AddTransient<MooredShipProviderService>();
        builder.Services.AddTransient<MooredShipOverviewCreationService>();
        builder.Services.AddTransient<CargoRemoverService>();
        builder.Services.AddTransient<MooredShipRemoverService>();
        builder.Services.AddTransient<ShippingProviderService>();
        builder.Services.AddTransient(provider =>
        {
            return new Func<string, HtmlResult>(
                path => ActivatorUtilities.CreateInstance<HtmlResult>(provider, path));
        });
        builder.Services.AddTransient(provider =>
        {
            return new Func<decimal, decimal, decimal, decimal, IPackingService>(
                (length, width, height, loadCapacity) => ActivatorUtilities.CreateInstance<PackingService>(provider, length, width, height, loadCapacity));
        });

        var app = builder.Build();
        app.UseStaticFiles();
        app.UseCors("MyPolicy");
        app.UseHttpsRedirection();
        app.MapControllers();
        app.Run();
    }
}