# Tilt setup for local orchestration.
# SmartHome stays local (JavaFX GUI), while Tilt manages DataService + Webhost.

allow_k8s_contexts('')

local_resource(
    name='data-service',
    serve_cmd='pwsh -NoProfile -Command "Set-Location smart-lodge/.assets/DataService; java -jar SmartQuartierDataService.jar"',
)

local_resource(
    name='webhost',
    serve_cmd='pwsh -NoProfile -Command "dotnet run --project AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj"',
    resource_deps=['data-service'],
)

print('SmartHome (JavaFX) is not started by Tilt by default.')
print('Run it locally in a separate terminal:')
print('  pwsh -NoProfile -Command "Set-Location smart-lodge/.assets/SmartHome; java -jar SmartHome.jar"')
