using System.Collections.Generic;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Authentication.Models;

public sealed record CurrentUserResponse(
  bool IsAuthenticated,
  string? UserName,
  string? DisplayName,
  IReadOnlyList<string> Roles
);
