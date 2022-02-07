dotnet build WriteBenchmark --configuration Release --framework net48
if ($LASTEXITCODE -eq 0) {
  Start-Process ./WriteBenchmark/bin/Release/net48/WriteBenchmark.exe -WorkingDirectory ./WriteBenchmark/bin/Release/net48 -NoNewWindow -Wait -ArgumentList "--filter *MultiThreadComparisonBetweenLibsTask*"
}