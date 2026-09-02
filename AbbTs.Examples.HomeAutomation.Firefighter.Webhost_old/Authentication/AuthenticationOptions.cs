namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Authentication;

public class AuthenticationOptions
{
  public const string SectionName = "Authentication";

  public string Authority { get; set; } = string.Empty;
  public string ClientId { get; set; } = string.Empty;
  public string ClientSecret { get; set; } = string.Empty;
}
