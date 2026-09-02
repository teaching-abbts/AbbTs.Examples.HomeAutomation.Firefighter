using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Authentication.Endpoints;

public static class AccountEndpoints
{
  public static RouteGroupBuilder MapAccountEndpoints(this WebApplication app)
  {
    var group = app.MapGroup("/account").WithTags("Account").AllowAnonymous();

    group.MapGet(
      "/login",
      (string? returnUrl, HttpContext context) =>
        Results.Challenge(
          new AuthenticationProperties { RedirectUri = returnUrl ?? "/" },
          [OpenIdConnectDefaults.AuthenticationScheme]
        )
    );

    group.MapGet(
      "/logout",
      async (HttpContext context) =>
      {
        await context.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return Results.SignOut(
          new AuthenticationProperties { RedirectUri = "/" },
          [OpenIdConnectDefaults.AuthenticationScheme]
        );
      }
    );

    group
      .MapGet("/user", (HttpContext context) => Results.Ok(context.User.ToCurrentUserResponse()))
      .Produces<Models.CurrentUserResponse>(StatusCodes.Status200OK);

    return group;
  }
}
