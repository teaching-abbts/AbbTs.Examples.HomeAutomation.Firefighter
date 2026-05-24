using System;
using System.Threading;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

using Akka.Actor;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.History;

public static class HistoryEndpoint
{
  private static readonly TimeSpan AskTimeout = TimeSpan.FromSeconds(5);

  public static RouteHandlerBuilder MapSmartQuartierHistoryEndpoint(this WebApplication app)
  {
    return app.MapGet(
        "/smart-quartier/history",
        async (ActorSystem actorSystem, CancellationToken cancellationToken) =>
        {
          var analyticsActor = await actorSystem
            .ActorSelection(SmartQuartierAnalyticsActor.ActorPath)
            .ResolveOne(AskTimeout)
            .WaitAsync(cancellationToken);

          var response = await analyticsActor
            .Ask<SmartQuartierHistoryResponse>(new GetSmartQuartierHistory(), AskTimeout)
            .WaitAsync(cancellationToken);

          return Results.Ok(response);
        }
      )
      .WithName("GetSmartQuartierHistory")
      .WithTags("SmartQuartier")
      .Produces<SmartQuartierHistoryResponse>(StatusCodes.Status200OK);
  }
}
