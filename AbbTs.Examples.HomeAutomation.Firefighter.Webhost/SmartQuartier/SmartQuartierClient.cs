using System.Text.Json;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier;

public interface ISmartQuartierClient
{
    Task<SmartQuartierHistoryResponse> GetHistoryDataAsync(CancellationToken cancellationToken);
}

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

    private static JsonSerializerOptions CreateJsonOptions()
    {
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new SmartQuartierTimestampJsonConverter());
        return options;
    }
}
