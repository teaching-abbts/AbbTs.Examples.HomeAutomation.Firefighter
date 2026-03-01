using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSmartQuartier(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddOptions<SmartQuartierOptions>()
            .Bind(configuration.GetSection(SmartQuartierOptions.SectionName))
            .Validate(
                options => Uri.TryCreate(options.BaseAddress, UriKind.Absolute, out _),
                $"{SmartQuartierOptions.SectionName}:BaseAddress must be a valid absolute URI.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.HistoryPath),
                $"{SmartQuartierOptions.SectionName}:HistoryPath must not be empty.")
            .Validate(
                options => !string.IsNullOrWhiteSpace(options.StatisticPath),
                $"{SmartQuartierOptions.SectionName}:StatisticPath must not be empty.")
            .ValidateOnStart();

        services
            .AddHttpClient<ISmartQuartierClient, SmartQuartierClient>((serviceProvider, httpClient) =>
            {
                var options = serviceProvider
                    .GetRequiredService<Microsoft.Extensions.Options.IOptions<SmartQuartierOptions>>()
                    .Value;

                httpClient.BaseAddress = new Uri(options.BaseAddress);
                httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
            })
            .SetHandlerLifetime(TimeSpan.FromMinutes(5));

        return services;
    }
}