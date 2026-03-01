using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Services;

public interface ISmartQuartierClient
{
    Task<SmartQuartierHistoryResponse> GetHistoryDataAsync(CancellationToken cancellationToken);

    Task<SmartQuartierStatisticResponse> GetStatisticDataAsync(CancellationToken cancellationToken);
}