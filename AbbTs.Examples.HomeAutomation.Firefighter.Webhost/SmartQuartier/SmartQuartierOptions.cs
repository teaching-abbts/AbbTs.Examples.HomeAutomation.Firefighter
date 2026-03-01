namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier;

public sealed class SmartQuartierOptions
{
    public const string SectionName = "SmartQuartier";

    public string BaseAddress { get; init; } = "http://127.0.0.1:11001/";

    public string HistoryPath { get; init; } = "smart-quartier/data-service/history";

    public int TimeoutSeconds { get; init; } = 10;
}