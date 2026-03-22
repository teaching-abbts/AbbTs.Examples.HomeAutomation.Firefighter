using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors.Messages;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

using Akka.Actor;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Actors;

public sealed class SmartQuartierAnalyticsActor : ReceiveActor
{
    public const string ActorName = "smart-quartier-analytics";
    public const string ActorPath = "/user/" + ActorName;
    private static readonly TimeSpan ClientTimeout = TimeSpan.FromSeconds(5);

    private readonly ISmartQuartierClient _smartQuartierClient;
    private readonly SmartQuartierOptions _options;

    public SmartQuartierAnalyticsActor(
        ISmartQuartierClient smartQuartierClient,
        SmartQuartierOptions options)
    {
        _smartQuartierClient = smartQuartierClient;
        _options = options;

        ReceiveAsync<GetSmartQuartierHistory>(async message =>
        {
            var replyTo = Sender;
            using var cts = new CancellationTokenSource(ClientTimeout);
            var response = await _smartQuartierClient.GetHistoryDataAsync(cts.Token);
            replyTo.Tell(ApplyEventLimit(response, message.RequestedEventLimit));
        });
    }

    private SmartQuartierHistoryResponse ApplyEventLimit(
        SmartQuartierHistoryResponse history,
        int? requestedEventLimit)
    {
        var effectiveLimit = _options.HistoryEventsLimit;

        if (requestedEventLimit is > 0)
        {
            effectiveLimit = Math.Min(effectiveLimit, requestedEventLimit.Value);
        }

        var limitedEvents = history.Events
            .OrderByDescending(item => item.TimeStamp)
            .Take(effectiveLimit)
            .ToArray();

        return history with
        {
            Events = limitedEvents,
        };
    }
}
