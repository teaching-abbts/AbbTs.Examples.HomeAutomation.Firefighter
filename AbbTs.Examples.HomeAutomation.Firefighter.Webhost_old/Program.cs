using System;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Configuration;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Authentication.Endpoints;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Endpoints;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Endpoints.About;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.NSwag;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    builder.Services.AddFirefighterHouses();
    builder.Services.AddSignalR();

    if (builder.Environment.EnvironmentName != "NSWAG")
    {
      builder.SetupSpaMiddleware(appSettings);
    }

    var app = builder.Build();

    app.UseNSwag();

    app.MapAccountEndpoints();
    app.MapAboutEndpoint();
    app.MapHousesEndpoints();
    app.MapHub<HouseControlHub>("/hubs/houses");

    if (app.Environment.EnvironmentName != "NSWAG")
    {
      app.MapSinglePageApps(appSettings);
    }

    await app.RunAsync();
  }
}
