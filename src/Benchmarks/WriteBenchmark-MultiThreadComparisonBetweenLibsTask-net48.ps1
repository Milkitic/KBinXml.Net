dotnet build WriteBenchmark --configuration Release --framework net48
if ($LASTEXITCODE -eq 0) {
  Start-Process dotnet -WorkingDirectory ./WriteBenchmark/bin/Release/net48 -NoNewWindow -Wait -ArgumentList "WriteBenchmark.dll --filter *MultiThreadComparisonBetweenLibsTask*"
}