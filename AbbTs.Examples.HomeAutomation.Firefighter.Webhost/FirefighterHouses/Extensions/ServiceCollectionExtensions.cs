using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Services;

using Akka.Actor;
using Akka.Hosting;

using Microsoft.Extensions.DependencyInjection;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.FirefighterHouses.Extensions;

public static class ServiceCollectionExtensions
{
  private const string ActorSystemName = "firefighter-houses";

  public static IServiceCollection AddFirefighterHouses(this IServiceCollection services)
  {
    services.AddAkka(
      ActorSystemName,
      (builder, _) =>
      {
        builder
          .AddHocon(
            """
            akka.persistence.journal.plugin = "akka.persistence.journal.inmem"
            akka.persistence.snapshot-store.plugin = "akka.persistence.no-snapshot-store"
            """,
            HoconAddMode.Prepend
          )
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

    services.AddSingleton<IHouseControlGateway, HouseControlGateway>();

    return services;
  }
}
