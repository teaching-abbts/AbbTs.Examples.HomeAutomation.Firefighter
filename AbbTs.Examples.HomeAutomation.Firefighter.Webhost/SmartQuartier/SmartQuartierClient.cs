namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier;

public interface ISmartQuartierClient
{
    Task<SmartQuartierResponse> GetHistoryAsync(CancellationToken cancellationToken);
}

public sealed class SmartQuartierClient(HttpClient httpClient, Microsoft.Extensions.Options.IOptions<SmartQuartierOptions> options) : ISmartQuartierClient
{
    private readonly SmartQuartierOptions _options = options.Value;

    public async Task<SmartQuartierResponse> GetHistoryAsync(CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, _options.HistoryPath);
        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var contentType = response.Content.Headers.ContentType?.ToString() ?? "application/json";

        return new SmartQuartierResponse((int)response.StatusCode, contentType, content);
    }
}

public sealed record SmartQuartierResponse(int StatusCode, string ContentType, string Content);