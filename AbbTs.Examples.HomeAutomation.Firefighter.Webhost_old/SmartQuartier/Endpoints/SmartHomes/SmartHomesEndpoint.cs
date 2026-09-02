using System.Collections.Generic;
using System.Threading;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.SmartHomes;

public static class SmartHomesEndpoint
{
  public static RouteGroupBuilder MapSmartHomesEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/api/smart-homes").WithTags("SmartHomes");

    group
      .MapGet(
        "/",
        async (ISmartHomeGateway gateway, CancellationToken cancellationToken) =>
        {
          var response = await gateway.GetSmartHomesAsync(cancellationToken);
          return Results.Ok(response);
        }
      )
      .WithName("GetSmartHomes")
      .Produces<IReadOnlyList<SmartHomeSummary>>(StatusCodes.Status200OK);

    group
      .MapGet(
        "/{smartHomeId}",
        async (
          string smartHomeId,
          ISmartHomeGateway gateway,
          CancellationToken cancellationToken
        ) =>
        {
          var response = await gateway.GetSmartHomeAsync(smartHomeId, cancellationToken);
          return response is null ? Results.NotFound() : Results.Ok(response);
        }
      )
      .WithName("GetSmartHome")
      .Produces<SmartHomeDetails>(StatusCodes.Status200OK)
      .Produces(StatusCodes.Status404NotFound);

    return group;
  }
}
