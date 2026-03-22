# AbbTs.Examples.HomeAutomation.Firefighter

A tutoring project in NDS Software Engineering at the ABB Technikerschule in Baden, Switzerland.

This README documents how to run the Firefighter Webhost in this repository.

Scope note: setup instructions for smart-lodge are intentionally excluded here.

## Firefighter Webhost Prerequisites

Install the following tools before running the app:

1. .NET SDK 10.0.100 (or a compatible 10.0.x version)
2. VitePlus CLI <https://viteplus.dev/> as the central JavaScript toolchain manager

Version checks:

```bash
dotnet --version
vp --version
```

Reference sources:

- .NET SDK pin: `global.json`
- Frontend package manager pin: `AbbTs.Examples.HomeAutomation.Firefighter.Webhost/Apps/firefighter-dashboard/package.json` (`pnpm@10.32.1`, managed by VitePlus)
- SPA integration commands: `AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj` (`vp install`, `vp run build`)

## Quick Start (Firefighter Webhost)

1. Install frontend dependencies:

```bash
cd AbbTs.Examples.HomeAutomation.Firefighter.Webhost/Apps/firefighter-dashboard
vp install
```

1. Start backend (ASP.NET Core Webhost):

```bash
dotnet run --project AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj
```

1. In a second terminal, start the frontend dev server:

```bash
cd AbbTs.Examples.HomeAutomation.Firefighter.Webhost/Apps/firefighter-dashboard
vp run dev
```

Local URLs:

- Backend: <http://localhost:5099>
- Backend HTTPS: <https://localhost:7118>
- Frontend dev server: <http://localhost:3000>
- Swagger: <http://localhost:5099/swagger>

## Runtime Dependency

The Webhost expects a SmartQuartier service endpoint at:

- <http://127.0.0.1:11001/>

If this service is unavailable, related API calls in the Webhost will fail or time out.

## Useful Commands

Backend:

```bash
dotnet build AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj
dotnet run --project AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj
```

Frontend:

```bash
cd AbbTs.Examples.HomeAutomation.Firefighter.Webhost/Apps/firefighter-dashboard
vp install
vp run dev
vp run build
vp run type-check
vp run lint
```

## Unified Local Runtime (SmartHome + DataService + Webhost)

Use the root script to start all required local processes:

```powershell
./run.ps1 -Action start
```

This starts:

- `smart-lodge/.assets/SmartHome/SmartHome.jar` (multiple instances based on config)
- `smart-lodge/.assets/DataService/SmartQuartierDataService.jar`
- `AbbTs.Examples.HomeAutomation.Firefighter.Webhost`

Process IDs are tracked in `.run/processes.json`.

Smart-home instance definitions are read from:

- `build/smart-homes.json`

The script is a thin wrapper around Cake Frosting targets:

- `Run-Start`
- `Run-Stop`
- `Run-Status`

Status and shutdown:

```powershell
./run.ps1 -Action status
./run.ps1 -Action stop
```

The stop action first tries graceful window close (where supported), then force-terminates remaining processes.

## Optional Tilt Workflow

An optional `Tiltfile` is included to run `data-service` and `webhost` as local resources:

```powershell
tilt up
```

Because SmartHome is JavaFX-based, it is intentionally kept outside container workflows. Start SmartHome separately:

```powershell
pwsh -NoProfile -Command "Set-Location smart-lodge/.assets/SmartHome; java -jar SmartHome.jar"
```

Shutdown with Tilt:

```powershell
tilt down
```

## SmartHome WebSocket Integration (Webhost)

The Webhost now supports the SmartHome gateway WebSocket protocol directly.

### Endpoints

- SmartHome -> Webhost: `ws://127.0.0.1:5099/smart-home/data`
- Dashboard client -> Webhost: `ws://127.0.0.1:5099/smart-home/ws`

### SmartHome Configuration

In `smart-lodge/.assets/SmartHome/SmartHome.conf`, add a ServiceProvider entry so SmartHome connects to the Webhost too:

```conf
SERVICE_PROVIDER = Firefighter; 127.0.0.1:5099
```

You can keep the DataService registration in parallel:

```conf
SERVICE_PROVIDER = DataService; 127.0.0.1:11000
SERVICE_PROVIDER = Firefighter; 127.0.0.1:5099
```

### Dashboard WebSocket Command Format

Send JSON messages to `/smart-home/ws`:

```json
{ "messageType": "get state" }
```

```json
{ "messageType": "get measurement" }
```

```json
{
  "messageType": "send command",
  "payload": {
    "device": "LightControl",
    "command": "setpoint",
    "value": "174"
  }
}
```

Supported `send command` targets (from the SmartHome protocol docs/source):

- `LightControl` with `on`, `off`, `setpoint`
- `HeatingControl` with `on`, `off`, `setpoint`
- `AlarmControl` with `on`, `off`
- `Door` with `open`, `close`
- `Display` with value format `line1;line2`

### Dashboard WebSocket Events

The Webhost forwards SmartHome messages to `/smart-home/ws` as envelopes:

```json
{
  "messageType": "send state",
  "payload": { "gateway": "readandwrite", "lightControl": "on" },
  "receivedAtUtc": "2026-03-22T12:00:00.0000000Z"
}
```

System and outbound tracing events are also emitted:

- `system status` with `{ "smartHomeConnected": true|false }`
- `outbound get state`, `outbound get measurement`, `outbound send command`
