[CmdletBinding()]
param(
  [ValidateSet("start", "stop", "status")]
  [string]$Action = "start"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$target = switch ($Action) {
  "start" { "Run-Start" }
  "stop" { "Run-Stop" }
  "status" { "Run-Status" }
  default { throw "Unknown action '$Action'." }
}

$buildProject = Join-Path $PSScriptRoot "build/Build.csproj"

& dotnet run --project $buildProject --target $target -- --repo-root $PSScriptRoot
if ($LASTEXITCODE -ne 0) {
  throw "Cake Frosting task '$target' failed with exit code $LASTEXITCODE."
}
