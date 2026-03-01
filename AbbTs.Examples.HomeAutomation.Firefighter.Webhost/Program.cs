using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Endpoints.About;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.NSwag;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.History;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.Statistic;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Extensions;

using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.Domain.Models;
using Mumrich.SpaDevMiddleware.Extensions;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var versionInfo = builder.Services.RegisterVersionInfo(typeof(Program).Assembly);
        var appSettings =
            builder.Configuration.Get<AppSettings>()
            ?? throw new InvalidOperationException("Failed to load application settings.");

        builder.Services.SetupNSwag(versionInfo);

        if (builder.Environment.EnvironmentName != "NSWAG")
        {
            builder.SetupSpaMiddleware(appSettings);
        }

        builder.Services.AddSmartQuartier(builder.Configuration);

        var app = builder.Build();

        app.UseNSwag();
        app.MapSmartQuartierHistoryEndpoint();
        app.MapSmartQuartierStatisticEndpoint();

        app.MapAboutEndpoint();

        if (app.Environment.EnvironmentName != "NSWAG")
        {
            app.MapSinglePageApps(appSettings);
        }

        await app.RunAsync();
    }
}

public class AppSettings : ISpaMiddlewareSettings
{
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
    public string BasePublicPath { get; set; } = Directory.GetCurrentDirectory();
}
