using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddOptions<SmartQuartierOptions>()
    .Bind(builder.Configuration.GetSection(SmartQuartierOptions.SectionName))
    .Validate(
        options => Uri.TryCreate(options.BaseAddress, UriKind.Absolute, out _),
        $"{SmartQuartierOptions.SectionName}:BaseAddress must be a valid absolute URI.")
    .Validate(
        options => !string.IsNullOrWhiteSpace(options.HistoryPath),
        $"{SmartQuartierOptions.SectionName}:HistoryPath must not be empty.")
    .ValidateOnStart();

builder.Services
    .AddHttpClient<ISmartQuartierClient, SmartQuartierClient>((serviceProvider, httpClient) =>
    {
        var options = serviceProvider
            .GetRequiredService<Microsoft.Extensions.Options.IOptions<SmartQuartierOptions>>()
            .Value;

        httpClient.BaseAddress = new Uri(options.BaseAddress);
        httpClient.Timeout = TimeSpan.FromSeconds(options.TimeoutSeconds);
    })
    .SetHandlerLifetime(TimeSpan.FromMinutes(5));

var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.MapGet("/smart-quartier/history", async (ISmartQuartierClient client, CancellationToken cancellationToken) =>
{
    var response = await client.GetHistoryDataAsync(cancellationToken);
    return Results.Ok(response);
});

await app.RunAsync();
