using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Endpoints.About;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.GitVersion.Extensions;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.NSwag;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.History;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Endpoints.Statistic;
using AbbTs.Examples.HomeAutomation.Firefighter.Webhost.SmartQuartier.Extensions;

var builder = WebApplication.CreateBuilder(args);
var versionInfo = builder.Services.RegisterVersionInfo(typeof(Program).Assembly);

builder.Services.SetupNSwag(versionInfo);
builder.Services.AddSmartQuartier(builder.Configuration);

var app = builder.Build();

app.UseNSwag();
app.MapSmartQuartierHistoryEndpoint();
app.MapSmartQuartierStatisticEndpoint();

app.MapAboutEndpoint();
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

await app.RunAsync();
