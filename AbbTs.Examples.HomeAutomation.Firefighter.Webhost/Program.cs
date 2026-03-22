using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Configuration;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Endpoints.About;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.NSwag;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.History;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.SmartHomes;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.Statistic;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.WebSocket;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

using Akka.Actor;
using Akka.Hosting;

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
        builder.Services.AddSignalR();

        builder.Services.AddAkka("firefighter-system", configurationBuilder =>
        {
            configurationBuilder.WithActors((actorSystem, actorRegistry) =>
            {
                actorSystem.ActorOf(Props.Create(() => new SmartHomeManagerActor()), SmartHomeManagerActor.ActorName);
            });
        });

        var app = builder.Build();

        app.UseWebSockets();

        app.UseNSwag();
        app.MapSmartQuartierHistoryEndpoint();
        app.MapSmartQuartierStatisticEndpoint();
        app.MapSmartHomesEndpoints();
        app.MapSmartHomeGatewayEndpoints();
        app.MapHub<SmartHomeHub>("/hubs/smart-homes");

        app.MapAboutEndpoint();

        if (app.Environment.EnvironmentName != "NSWAG")
        {
            app.MapSinglePageApps(appSettings);
        }

        await app.RunAsync();
    }
}
