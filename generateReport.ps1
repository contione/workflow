Remove-Item -Recurse -Force "UnitTests/TestResults" -ErrorAction SilentlyContinue
Remove-Item -Recurse -Force "UnitTests/TestResultsCoverageReport" -ErrorAction SilentlyContinue
 
$output = dotnet test --collect:"XPlat Code Coverage"
$lastLine = $output | Select-Object -Last 1
 
Write-Host $lastLine -ForegroundColor Blue
 
$coverageFile = $lastLine.Trim()
 
if (-not $coverageFile -or -not (Test-Path -Path $coverageFile)) 
{
    exit 1
}
 
Write-Host "coverageFile path: $coverageFile" -ForegroundColor Blue
 
reportgenerator -reports:$coverageFile -targetdir:"UnitTests/TestResultsCoverageReport" -reporttypes:Html
 
Write-Host "== Good day chief! ==" -ForegroundColor Blue
start UnitTests\TestResultsCoverageReport\index.html