# AbbTs.Examples.HomeAutomation.Firefighter

A tutoring project in NDS Software Engineering at the ABB Technikerschule in Baden, Switzerland.

This README documents how to run the Firefighter Webhost in this repository.

Scope note: setup instructions for smart-lodge are intentionally excluded here.

## Firefighter Webhost Prerequisites

Install the following tools before running the app:

1. .NET SDK 10.0.100 (or a compatible 10.0.x version)
2. VitePlus CLI <https://viteplus.dev/> as the central JavaScript toolchain manager
3. A Java runtime with bundled JavaFX on your PATH as `java` (e.g. Azul Zulu OpenJDK FX 21) — required by Tilt to start the smart-lodge `DataService` and `SmartHome` Java processes. `SmartHome` is compiled with JDK 11 but needs a JavaFX-enabled JRE/JDK to run.
4. Docker Desktop with Docker Compose v2 on your PATH — required for the Authentik stack

On Windows, Docker Desktop must be allowed to share the drive containing this repository. If Compose reports that `infra/authentik/blueprints` is not shared, open Docker Desktop settings and enable file sharing or WSL access for that drive, then retry the start command.

Version checks:

```bash
dotnet --version
vp --version
java --version
```

Reference sources:

- .NET SDK pin: `global.json`
- Frontend package manager pin: `AbbTs.Examples.HomeAutomation.Firefighter.Webhost/Apps/firefighter-dashboard/package.json` (`pnpm@10.32.1`, managed by VitePlus)
- SPA integration commands: `AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj` (`vp install`, `vp run build`)

## Quick Start

The recommended local setup uses Tilt to start Authentik, the SmartQuartier
DataService, the Firefighter Webhost, and the three sample SmartHomes together.

1. Verify the prerequisites listed above.

2. Start the complete local runtime:

```powershell
tilt up
```

3. Open the dashboard at <http://localhost:5099> and sign in with one of the
  development accounts listed in [Local Credentials](#local-credentials).

Tilt starts the DataService through
`smart-lodge/.assets/DataService/start-data-service.ps1`. The launcher creates
the CSV persistence files when they are missing, which allows the DataService
history endpoint to work on a fresh checkout.

To inspect resource status:

```powershell
tilt get uiresources
```

Open the Tilt dashboard at <http://localhost:10350>. Stop the stack with:

```powershell
tilt down
```

## Local Credentials

These credentials are for local development only and are defined in
`infra/authentik/.env`.

| Account                  | Username      | Password                      | Role          |
| ------------------------ | ------------- | ----------------------------- | ------------- |
| Firefighter dashboard    | `citizen`     | `dev-only-demo-user-password` | Viewer        |
| Firefighter dashboard    | `firefighter` | `dev-only-demo-user-password` | Operator      |
| Authentik administration | `akadmin`     | `dev-only-akadmin-password`   | Administrator |

Use `citizen` or `firefighter` at the Firefighter dashboard. Use `akadmin` at
the Authentik administration interface at <http://localhost:9000>.

Never reuse these development credentials outside the local environment.

## Local Endpoints

Use these hostnames and ports while the local Tilt stack is running:

| Service                     | Address                                            | Purpose                                    |
| --------------------------- | -------------------------------------------------- | ------------------------------------------ |
| Firefighter dashboard       | <http://localhost:5099>                            | Main application                           |
| Firefighter Webhost (HTTPS) | <https://localhost:7118>                           | Main application over HTTPS                |
| Swagger                     | <http://localhost:5099/swagger>                    | Webhost API documentation                  |
| Authentik administration    | <http://localhost:9000>                            | Manage users, groups, providers, and flows |
| Authentik OAuth authority   | <http://localhost:9000/application/o/firefighter/> | Sign-in authority used by the Webhost      |
| Frontend Vite dev server    | <http://localhost:3000>                            | Direct SPA development server              |
| SmartQuartier DataService   | <http://127.0.0.1:11001>                           | History, statistics, and forecast API      |
| Tilt dashboard              | <http://localhost:10350>                           | Runtime and resource status                |

## Manual Webhost Development

1. Install frontend dependencies:

```bash
cd AbbTs.Examples.HomeAutomation.Firefighter.Webhost/Apps/firefighter-dashboard
vp install
```

2. Start the backend (ASP.NET Core Webhost):

```bash
dotnet run --project AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj
```

3. In a second terminal, start the frontend dev server:

```bash
cd AbbTs.Examples.HomeAutomation.Firefighter.Webhost/Apps/firefighter-dashboard
vp run dev
```

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

## Artifact Build

Build runtime artifacts into `.artifacts`:

```powershell
./build.ps1 --target Artifacts-Build
```

```bash
./build.sh --target Artifacts-Build
```

The `.artifacts` folder contains:

- `webhost` (published ASP.NET Core app via `dotnet publish`)
- `DataService` (copied from `smart-lodge/.assets/DataService`)
- `SmartHome` (copied from `smart-lodge/.assets/SmartHome`)
- `build/smart-homes.json` (runtime smart-home instance config)

## Local Runtime With Tilt

To pass custom arguments to Tilt using the convenience wrappers:

```powershell
./run.ps1 --port 10351
```

```bash
./run.sh --port 10351
```

The SmartHome instances are generated from `build/smart-homes.json` and run
with isolated configuration directories under `.run/smarthomes`.

## SmartHome WebSocket Integration (Webhost)

The Webhost now supports the SmartHome gateway WebSocket protocol directly.

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

- `system status` with `{ "smartHomeConnected": true|false }`
- `outbound get state`, `outbound get measurement`, `outbound send command`
