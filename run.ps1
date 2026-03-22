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
$dataServiceDir = Join-Path $repoRoot "smart-lodge/.assets/DataService"
$webhostProject = Join-Path $repoRoot "AbbTs.Examples.HomeAutomation.Firefighter.Webhost/AbbTs.Examples.HomeAutomation.Firefighter.Webhost.csproj"

$components = @(
	@{
		Name = "smart-home"
		FilePath = "java"
		Arguments = @("-jar", "SmartHome.jar")
		WorkingDirectory = $smartHomeDir
	},
	@{
		Name = "data-service"
		FilePath = "java"
		Arguments = @("-jar", "SmartQuartierDataService.jar")
		WorkingDirectory = $dataServiceDir
	},
	@{
		Name = "webhost"
		FilePath = "dotnet"
		Arguments = @("run", "--project", $webhostProject)
		WorkingDirectory = $repoRoot
	}
)

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
