using Mumrich.SpaDevMiddleware.Domain.Contracts;
using Mumrich.SpaDevMiddleware.Domain.Models;

namespace AbbTs.Examples.HomeAutomation.Firefighter.Webhost.Configuration;

public class AppSettings : ISpaMiddlewareSettings
{
    public Dictionary<string, SpaSettings> SinglePageApps { get; set; } = new();
    public string BasePublicPath { get; set; } = Directory.GetCurrentDirectory();
}
