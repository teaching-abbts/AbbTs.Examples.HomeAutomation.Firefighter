using System;
using System.Threading.Tasks;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Configuration;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Authentication;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Authentication.Endpoints;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Endpoints.About;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.NSwag;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.History;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.SmartHomes;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.Statistic;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.WebSocket;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

using Akka.Actor;
using Akka.Hosting;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

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
      // builder.Services.AddFirefighterAuthentication(builder.Configuration);
    }

    var app = builder.Build();

    // if (app.Environment.EnvironmentName != "NSWAG")
    // {
    //   app.UseAuthentication();
    //   app.UseAuthorization();
    // }

    // Mapped unconditionally so NSwag can discover it and generate a typed client.
    app.MapAccountEndpoints();

    app.UseNSwag();

    app.MapAboutEndpoint();

    if (app.Environment.EnvironmentName != "NSWAG")
    {
      app.MapSinglePageApps(appSettings);
    }

    await app.RunAsync();
  }
}
