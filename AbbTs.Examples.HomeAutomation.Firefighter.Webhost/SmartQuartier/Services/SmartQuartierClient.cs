using System.Text.Json;

using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

public sealed class SmartQuartierClient(HttpClient httpClient, Microsoft.Extensions.Options.IOptions<SmartQuartierOptions> options) : ISmartQuartierClient
{
    private readonly SmartQuartierOptions _options = options.Value;
    private static readonly JsonSerializerOptions JsonOptions = CreateJsonOptions();

    public async Task<SmartQuartierHistoryResponse> GetHistoryDataAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _options.HistoryPath);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<SmartQuartierHistoryResponse>(stream, JsonOptions, cancellationToken);

        return payload ?? throw new JsonException("Received empty payload from SmartQuartier history endpoint.");
    }

    public async Task<SmartQuartierStatisticResponse> GetStatisticDataAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _options.StatisticPath);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var payload = await JsonSerializer.DeserializeAsync<SmartQuartierStatisticResponse>(stream, JsonOptions, cancellationToken);

        return payload ?? throw new JsonException("Received empty payload from SmartQuartier statistic endpoint.");
    }

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new SmartQuartierTimestampJsonConverter());
        return options;
    }
}