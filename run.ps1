[CmdletBinding()]
param(
  [Parameter(ValueFromRemainingArguments = $true)]
  [string[]]$TiltArguments
)

& tilt up --file (Join-Path $PSScriptRoot "Tiltfile") @TiltArguments
exit $LASTEXITCODE
