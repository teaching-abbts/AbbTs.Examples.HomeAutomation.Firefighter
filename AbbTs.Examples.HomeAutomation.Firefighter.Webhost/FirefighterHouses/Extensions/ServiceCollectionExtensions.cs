using System;
using System.IO;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;

using Akka.Actor;
using Akka.Hosting;
using Akka.Persistence.Sql.Hosting;

using LinqToDB;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Extensions;

public static class ServiceCollectionExtensions
{
  private const string ActorSystemName = "firefighter-houses";

  public static IServiceCollection AddFirefighterHouses(this IServiceCollection services)
  {
    services.AddAkka(
      ActorSystemName,
      (builder, serviceProvider) =>
      {
        var environment = serviceProvider.GetRequiredService<IHostEnvironment>();
        var dataDirectory = Path.Combine(environment.ContentRootPath, "App_Data");
        Directory.CreateDirectory(dataDirectory);
        var dbPath = Path.Combine(dataDirectory, "firefighter-houses.db");

        builder
          .WithSqlPersistence(connectionString: $"Data Source={dbPath}", providerName: ProviderName.SQLite)
          .WithActors(
            (system, registry) =>
            {
              var manager = system.ActorOf(
                Props.Create(() => new HouseManagerActor()),
                HouseManagerActor.ActorName
              );
              registry.Register<HouseManagerActor>(manager);
            }
          );
      }
    );

    services.AddSingleton<IHouseReadModel, HouseReadModel>();
    services.AddSingleton<IHouseControlGateway, HouseControlGateway>();

    return services;
  }
}
