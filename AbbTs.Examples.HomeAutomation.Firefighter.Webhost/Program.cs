using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Endpoints.About;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.NSwag;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier;

var builder = WebApplication.CreateBuilder(args);
var versionInfo = builder.Services.RegisterVersionInfo(typeof(Program).Assembly);

builder.Services.SetupNSwag(versionInfo);

builder.Services
    .AddOptions<SmartQuartierOptions>()
    .Bind(builder.Configuration.GetSection(SmartQuartierOptions.SectionName))
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

app.UseNSwag();
app.MapGet("/smart-quartier/history", async (ISmartQuartierClient client, CancellationToken cancellationToken) =>
{
    var response = await client.GetHistoryDataAsync(cancellationToken);
    return Results.Ok(response);
});

app.MapGet("/smart-quartier/statistic", async (ISmartQuartierClient client, CancellationToken cancellationToken) =>
{
    var response = await client.GetStatisticDataAsync(cancellationToken);
    return Results.Ok(response);
});

app.MapAboutEndpoint();
app.MapGet("/", () => Results.Redirect("/swagger"));

await app.RunAsync();
