[CmdletBinding()]
param(
  [ValidateSet("start", "stop", "status")]
  [string]$Action = "start"
)

Set-StrictMode -Version Latest
$ErrorActionPreference = "Stop"

$repoRoot = $PSScriptRoot
$stateDir = Join-Path $repoRoot ".run"
$pidFile = Join-Path $stateDir "processes.json"

$smartHomeDir = Join-Path $repoRoot "smart-lodge/.assets/SmartHome"
$smartHomeJarPath = Join-Path $smartHomeDir "SmartHome.jar"
$smartHomeInstancesRoot = Join-Path $stateDir "smarthomes"
$dataServiceDir = Join-Path $repoRoot "smart-lodge/.assets/DataService"
$webhostProject = Join-Path $repoRoot "AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj"

$smartHomeInstances = @(
  @{
    Name = "smart-home-1"
    BuildingId = "Haus-1"
    Owner = "Mike"
    X = 0
    Y = 0
  },
  @{
    Name = "smart-home-2"
    BuildingId = "Haus-2"
    Owner = "Anna"
    X = 10
    Y = 0
  },
  @{
    Name = "smart-home-3"
    BuildingId = "Haus-3"
    Owner = "Luca"
    X = 20
    Y = 0
  }
)

function Get-SmartHomeInstanceDirectory {
  param(
    [Parameter(Mandatory = $true)]
    [string]$InstanceName
  )

  return (Join-Path $smartHomeInstancesRoot $InstanceName)
}

function Set-ConfigValue {
  param(
    [Parameter(Mandatory = $true)]
    [string]$Config,
    [Parameter(Mandatory = $true)]
    [string]$Key,
    [Parameter(Mandatory = $true)]
    [string]$Value
  )

  return [System.Text.RegularExpressions.Regex]::Replace(
    $Config,
    "(?m)^\s*$([System.Text.RegularExpressions.Regex]::Escape($Key))\s*=\s*.*$",
    "${Key} = ${Value}")
}

function Initialize-SmartHomeInstances {
  Ensure-StateDirectory

  if (-not (Test-Path -LiteralPath $smartHomeJarPath)) {
    throw "SmartHome jar was not found at '$smartHomeJarPath'."
  }

  $templateConfigPath = Join-Path $smartHomeDir "SmartHome.conf"
  if (-not (Test-Path -LiteralPath $templateConfigPath)) {
    throw "SmartHome configuration template was not found at '$templateConfigPath'."
  }

  $templateConfig = Get-Content -LiteralPath $templateConfigPath -Raw

  foreach ($instance in $smartHomeInstances) {
    $instanceDir = Get-SmartHomeInstanceDirectory -InstanceName $instance.Name
    if (-not (Test-Path -LiteralPath $instanceDir)) {
      New-Item -ItemType Directory -Path $instanceDir -Force | Out-Null
    }

    $instanceConfig = $templateConfig
    $instanceConfig = Set-ConfigValue -Config $instanceConfig -Key "BUILDING_ID" -Value $instance.BuildingId
    $instanceConfig = Set-ConfigValue -Config $instanceConfig -Key "OWNER" -Value $instance.Owner
    $instanceConfig = Set-ConfigValue -Config $instanceConfig -Key "X_COORDINATE" -Value ([string]$instance.X)
    $instanceConfig = Set-ConfigValue -Config $instanceConfig -Key "Y_COORDINATE" -Value ([string]$instance.Y)

    Set-Content -LiteralPath (Join-Path $instanceDir "SmartHome.conf") -Value $instanceConfig -Encoding UTF8
  }
}

function Get-Components {
  $components = @(
    @{
      Name = "data-service"
      FilePath = "java"
      Arguments = @("-jar", "SmartQuartierDataService.jar")
      WorkingDirectory = $dataServiceDir
    },
    @{
      Name = "webhost"
      FilePath = "dotnet"
      Arguments = @("run", "--project", $webhostProject, "--", "--urls", "http://localhost:5099")
      WorkingDirectory = $repoRoot
      WaitForPort = 5099
      WaitTimeoutSeconds = 45
    }
  )

  foreach ($instance in $smartHomeInstances) {
    $components += @{
      Name = $instance.Name
      FilePath = "java"
      Arguments = @("-jar", $smartHomeJarPath)
      WorkingDirectory = (Get-SmartHomeInstanceDirectory -InstanceName $instance.Name)
    }
  }

  return $components
}

function Wait-ForTcpPort {
  param(
    [Parameter(Mandatory = $true)]
    [int]$Port,
    [Parameter(Mandatory = $true)]
    [int]$TimeoutSeconds
  )

  $deadline = (Get-Date).AddSeconds($TimeoutSeconds)
  while ((Get-Date) -lt $deadline) {
    $listener = Get-NetTCPConnection -State Listen -LocalPort $Port -ErrorAction SilentlyContinue
    if ($listener) {
      return $true
    }

    Start-Sleep -Milliseconds 500
  }

  return $false
}

function Ensure-StateDirectory {
  if (-not (Test-Path -LiteralPath $stateDir)) {
    New-Item -ItemType Directory -Path $stateDir | Out-Null
  }
}

function Ensure-ExecutableAvailable {
  param(
    [Parameter(Mandatory = $true)]
    [string]$Command
  )

  if (-not (Get-Command $Command -ErrorAction SilentlyContinue)) {
    throw "Required command '$Command' was not found in PATH."
  }
}

function Get-TrackedProcesses {
  if (-not (Test-Path -LiteralPath $pidFile)) {
    return @()
  }

  $json = Get-Content -LiteralPath $pidFile -Raw
  if ([string]::IsNullOrWhiteSpace($json)) {
    return @()
  }

  $data = $json | ConvertFrom-Json
  if ($data -is [System.Array]) {
    return @($data)
  }

  return @($data)
}

function Save-TrackedProcesses {
  param(
    [Parameter(Mandatory = $true)]
    [object[]]$Processes
  )

  Ensure-StateDirectory
  $Processes | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath $pidFile -Encoding UTF8
}

function Remove-TrackedProcessesFile {
  if (Test-Path -LiteralPath $pidFile) {
    Remove-Item -LiteralPath $pidFile -Force
  }
}

function Start-Components {
  Initialize-SmartHomeInstances
  $components = @(Get-Components)

  foreach ($component in $components) {
    Ensure-ExecutableAvailable -Command $component.FilePath

    if (-not (Test-Path -LiteralPath $component.WorkingDirectory)) {
      throw "Working directory '$($component.WorkingDirectory)' does not exist."
    }
  }

  $alreadyRunning = @(Get-TrackedProcesses | Where-Object {
      Get-Process -Id $_.Pid -ErrorAction SilentlyContinue
    })

  if ($alreadyRunning.Count -gt 0) {
    Write-Host "There are already tracked processes running. Use .\\run.ps1 -Action stop first."
    return
  }

  $started = @()
  try {
    foreach ($component in $components) {
      $process = Start-Process -FilePath $component.FilePath -ArgumentList $component.Arguments -WorkingDirectory $component.WorkingDirectory -PassThru
      $started += [PSCustomObject]@{
        Name = $component.Name
        Pid = $process.Id
        StartTimeUtc = [DateTime]::UtcNow.ToString("o")
        WorkingDirectory = $component.WorkingDirectory
        Command = "$($component.FilePath) $($component.Arguments -join ' ')"
      }
      Write-Host "Started $($component.Name) (PID $($process.Id))"

      if ($component.ContainsKey("WaitForPort")) {
        $timeoutSeconds = if ($component.ContainsKey("WaitTimeoutSeconds")) { [int]$component.WaitTimeoutSeconds } else { 30 }
        $port = [int]$component.WaitForPort
        Write-Host "Waiting for $($component.Name) to listen on port $port..."

        if (-not (Wait-ForTcpPort -Port $port -TimeoutSeconds $timeoutSeconds)) {
          throw "$($component.Name) did not open port $port within $timeoutSeconds seconds."
        }

        Write-Host "$($component.Name) is listening on port $port."
      }
    }

    Save-TrackedProcesses -Processes $started
    Write-Host "All components started."
    Write-Host "Use .\\run.ps1 -Action status to inspect and .\\run.ps1 -Action stop to terminate all."
  }
  catch {
    Write-Warning "Startup failed: $($_.Exception.Message)"
    if ($started.Count -gt 0) {
      Stop-TrackedComponents -Tracked $started
    }
    throw
  }
}

function Stop-TrackedComponents {
  param(
    [Parameter(Mandatory = $true)]
    [object[]]$Tracked
  )

  foreach ($entry in $Tracked) {
    $process = Get-Process -Id $entry.Pid -ErrorAction SilentlyContinue
    if (-not $process) {
      Write-Host "$($entry.Name): PID $($entry.Pid) is not running."
      continue
    }

    if ($process.MainWindowHandle -ne 0) {
      $null = $process.CloseMainWindow()
      if ($process.WaitForExit(5000)) {
        Write-Host "$($entry.Name): closed gracefully (PID $($entry.Pid))."
        continue
      }
    }

    Stop-Process -Id $entry.Pid -Force
    Write-Host "$($entry.Name): terminated (PID $($entry.Pid))."
  }
}

function Stop-Components {
  $tracked = @(Get-TrackedProcesses)
  if ($tracked.Count -eq 0) {
    Write-Host "No tracked processes found."
    return
  }

  Stop-TrackedComponents -Tracked $tracked
  Remove-TrackedProcessesFile
  Write-Host "Stopped all tracked components."
}

function Show-Status {
  $tracked = @(Get-TrackedProcesses)
  if ($tracked.Count -eq 0) {
    Write-Host "No tracked processes found."
    return
  }

  foreach ($entry in $tracked) {
    $process = Get-Process -Id $entry.Pid -ErrorAction SilentlyContinue
    if ($process) {
      Write-Host "$($entry.Name): running (PID $($entry.Pid))"
    }
    else {
      Write-Host "$($entry.Name): stopped (PID $($entry.Pid))"
    }
  }
}

switch ($Action) {
  "start" { Start-Components }
  "stop" { Stop-Components }
  "status" { Show-Status }
  default { throw "Unknown action '$Action'." }
}
