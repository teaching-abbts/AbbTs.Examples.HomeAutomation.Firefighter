# Tilt setup for the complete local runtime.

allow_k8s_contexts('')

docker_compose('infra/authentik/docker-compose.yml')
dc_resource('authentik-db', labels=['authentik'])
dc_resource('authentik-redis', labels=['authentik'])
dc_resource('authentik-server', labels=['authentik'])
dc_resource('authentik-worker', labels=['authentik'])

local_resource(
    name='data-service',
    serve_cmd='powershell -NoProfile -ExecutionPolicy Bypass -File start-data-service.ps1',
    serve_dir='smart-lodge/.assets/DataService',
    labels=['smart-lodge'],
)

local_resource(
    name='webhost',
    serve_cmd='dotnet run --project AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj -- --urls http://localhost:5099',
    serve_dir='.',
    resource_deps=['data-service', 'authentik-server'],
    links=[link('http://localhost:5099', 'Webhost')],
    labels=['firefighter'],
    trigger_mode=TRIGGER_MODE_AUTO,
)

local_resource(
    name='smart-home-setup',
    cmd='dotnet run --project build/Build.csproj --target Run-Prepare-SmartHomes -- --repo-root .',
    deps=['build/smart-homes.json', 'smart-lodge/.assets/SmartHome/SmartHome.conf'],
    resource_deps=['data-service'],
    labels=['smart-lodge'],
)

local_resource(
    name='smart-home-1',
    serve_cmd='java -jar ../../../smart-lodge/.assets/SmartHome/SmartHome.jar',
    serve_dir='.run/smarthomes/smart-home-1',
    resource_deps=['smart-home-setup', 'data-service', 'webhost'],
    labels=['smart-lodge'],
)

local_resource(
    name='smart-home-2',
    serve_cmd='java -jar ../../../smart-lodge/.assets/SmartHome/SmartHome.jar',
    serve_dir='.run/smarthomes/smart-home-2',
    resource_deps=['smart-home-setup', 'data-service', 'webhost'],
    labels=['smart-lodge'],
)

local_resource(
    name='smart-home-3',
    serve_cmd='java -jar ../../../smart-lodge/.assets/SmartHome/SmartHome.jar',
    serve_dir='.run/smarthomes/smart-home-3',
    resource_deps=['smart-home-setup', 'data-service', 'webhost'],
    labels=['smart-lodge'],
)
