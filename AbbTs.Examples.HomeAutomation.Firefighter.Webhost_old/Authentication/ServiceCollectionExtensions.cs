using System;
using System.Linq;
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Authentication;

public static class ServiceCollectionExtensions
{
  public const string OperatorPolicy = "Operator";
  public const string RoleClaimType = "groups";

  public static IServiceCollection AddFirefighterAuthentication(
    this IServiceCollection services,
    IConfiguration configuration
  )
  {
    services
      .AddOptions<AuthenticationOptions>()
      .Bind(configuration.GetSection(AuthenticationOptions.SectionName))
      .Validate(
        options => Uri.TryCreate(options.Authority, UriKind.Absolute, out _),
        $"{AuthenticationOptions.SectionName}:Authority must be a valid absolute URI."
      )
      .Validate(
        options => !string.IsNullOrWhiteSpace(options.ClientId),
        $"{AuthenticationOptions.SectionName}:ClientId must not be empty."
      )
      .ValidateOnStart();

    var authOptions =
      configuration.GetSection(AuthenticationOptions.SectionName).Get<AuthenticationOptions>()
      ?? throw new InvalidOperationException("Failed to load Authentication settings.");

    services
      .AddAuthentication(options =>
      {
        options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = OpenIdConnectDefaults.AuthenticationScheme;
      })
      .AddCookie()
      .AddOpenIdConnect(options =>
      {
        options.Authority = authOptions.Authority;
        options.ClientId = authOptions.ClientId;
        options.ClientSecret = authOptions.ClientSecret;
        options.RequireHttpsMetadata = !authOptions.Authority.StartsWith(
          "http://",
          StringComparison.OrdinalIgnoreCase
        );
        options.ResponseType = OpenIdConnectResponseType.Code;
        options.SaveTokens = true;
        options.GetClaimsFromUserInfoEndpoint = true;
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("groups");
        options.TokenValidationParameters.RoleClaimType = RoleClaimType;
        options.TokenValidationParameters.NameClaimType = "name";
      });

    services.AddAuthorizationBuilder()
      .SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
      .AddPolicy(OperatorPolicy, policy => policy.RequireRole("operator"));

    return services;
  }

  public static Models.CurrentUserResponse ToCurrentUserResponse(
    this ClaimsPrincipal user
  )
  {
    if (user.Identity?.IsAuthenticated != true)
    {
      return new Models.CurrentUserResponse(false, null, null, []);
    }

    var roles = user.Claims.Where(c => c.Type == RoleClaimType).Select(c => c.Value).ToList();

    return new Models.CurrentUserResponse(
      true,
      user.Identity.Name,
      user.FindFirst("name")?.Value ?? user.Identity.Name,
      roles
    );
  }
}
