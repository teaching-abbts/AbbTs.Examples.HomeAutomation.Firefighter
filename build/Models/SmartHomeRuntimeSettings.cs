using System.Collections.Generic;

namespace Build.Models;

public sealed class SmartHomeRuntimeSettings
{
  public List<SmartHomeInstance> SmartHomes { get; init; } = [];
}

public sealed class SmartHomeInstance
{
  public required string Name { get; init; }
  public required string BuildingId { get; init; }
  public required string Owner { get; init; }
  public int X { get; init; }
  public int Y { get; init; }
}
