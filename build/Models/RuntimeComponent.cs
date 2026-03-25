namespace Build.Models;

public sealed class RuntimeComponent
{
    public required string Name { get; init; }
    public required string FileName { get; init; }
    public required string WorkingDirectory { get; init; }
    public required IReadOnlyList<string> Arguments { get; init; }
    public int? WaitForPort { get; init; }
    public int WaitTimeoutSeconds { get; init; } = 30;
}
