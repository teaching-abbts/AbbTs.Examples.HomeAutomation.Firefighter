namespace Build.Models;

public sealed class TrackedProcess
{
    public required string Name { get; init; }
    public required int Pid { get; init; }
    public required string StartTimeUtc { get; init; }
    public required string WorkingDirectory { get; init; }
    public required string Command { get; init; }
}
