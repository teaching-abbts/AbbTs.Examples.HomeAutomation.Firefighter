using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

using Akka.Actor;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.Statistic;

public static class StatisticEndpoint
{
    private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(5);

    public static RouteHandlerBuilder MapSmartQuartierStatisticEndpoint(this WebApplication app)
    {
        return app.MapGet("/smart-quartier/statistic", async (ActorSystem actorSystem, CancellationToken cancellationToken) =>
            {
                var analyticsActor = await actorSystem
                    .ActorSelection(SmartQuartierAnalyticsActor.ActorPath)
                    .ResolveOne(AskTimeout)
                    .WaitAsync(cancellationToken);

                var response = await analyticsActor
                    .Ask<SmartQuartierStatisticResponse>(
                        new GetSmartQuartierStatistic(),
                        AskTimeout)
                    .WaitAsync(cancellationToken);

                return Results.Ok(response);
            })
            .WithName("GetSmartQuartierStatistic")
            .WithTags("SmartQuartier")
            .Produces<SmartQuartierStatisticResponse>(StatusCodes.Status200OK);
    }
}