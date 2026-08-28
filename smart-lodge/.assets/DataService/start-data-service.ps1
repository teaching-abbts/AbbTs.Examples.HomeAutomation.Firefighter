$ErrorActionPreference = 'Stop'

New-Item -ItemType File -Force 'SmartQuartierMeasurements.csv' | Out-Null
New-Item -ItemType File -Force 'SmartQuartierEvents.csv' | Out-Null

& java -jar SmartQuartierDataService.jar
exit $LASTEXITCODE
