using System;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Extensions;

public static class ServiceCollectionExtensions
{
  public static IServiceCollection AddSmartQuartier(
    this IServiceCollection services,
    IConfiguration configuration
  )
  {
    services
      .AddOptions<SmartQuartierOptions>()
      .Bind(configuration.GetSection(SmartQuartierOptions.SectionName))
      .Validate(
        options => Uri.TryCreate(options.BaseAddress, UriKind.Absolute, out _),
        $"{SmartQuartierOptions.SectionName}:BaseAddress must be a valid absolute URI."
      )
      .Validate(
        options => !string.IsNullOrWhiteSpace(options.HistoryPath),
        $"{SmartQuartierOptions.SectionName}:HistoryPath must not be empty."
      )
      .Validate(
        options => !string.IsNullOrWhiteSpace(options.StatisticPath),
        $"{SmartQuartierOptions.SectionName}:StatisticPath must not be empty."
      )
      .Validate(
        options => options.HistoryEventsLimit > 0,
        $"{SmartQuartierOptions.SectionName}:HistoryEventsLimit must be greater than 0."
      )
      .ValidateOnStart();

    services
      .AddHttpClient<ISmartQuartierClient, SmartQuartierClient>(
        (serviceProvider, httpClient) =>
        {
          var options = serviceProvider
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<SmartQuartierOptions>>()
            .Value;

          httpClient.BaseAddress = new Uri(options.BaseAddress);
          httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
        }
      )
      .SetHandlerLifetime(TimeSpan.FromMinutes(5));

    services.AddSingleton<ISmartHomeGateway, SmartHomeGateway>();

    return services;
  }
}
