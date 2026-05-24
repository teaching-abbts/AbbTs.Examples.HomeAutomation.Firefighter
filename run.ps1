[CmdletBinding()]
param(
  [ValidateSet("start", "stop", "status")]
  [string]$Action = "start",
  [ValidateSet("local", "artifacts")]
  [string]$Mode = "local"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$target = switch ("${Mode}:${Action}") {
  "local:start" { "Run-Start" }
  "local:stop" { "Run-Stop" }
  "local:status" { "Run-Status" }
  "artifacts:start" { "Artifacts-Run-Start" }
  "artifacts:stop" { "Artifacts-Run-Stop" }
  "artifacts:status" { "Artifacts-Run-Status" }
  default { throw "Unsupported mode/action combination '$Mode/$Action'." }
}

$repoRoot = if (Test-Path (Join-Path $PSScriptRoot "build/Build.csproj")) {
  $PSScriptRoot
}
elseif (Test-Path (Join-Path $PSScriptRoot "../build/Build.csproj")) {
  (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
}
else {
  throw "Could not locate build/Build.csproj from '$PSScriptRoot'."
}

$buildProject = Join-Path $repoRoot "build/Build.csproj"

& dotnet run --project $buildProject --target $target -- --repo-root $repoRoot
if ($LASTEXITCODE -ne 0) {
  throw "Cake Frosting task '$target' failed with exit code $LASTEXITCODE."
}
