dotnet build ReadBenchmark --configuration Release --framework net48
if ($LASTEXITCODE -eq 0) {
  Start-Process dotnet -WorkingDirectory ./ReadBenchmark/bin/Release/net48 -NoNewWindow -Wait -ArgumentList "ReadBenchmark.dll --filter *SingleThreadComparisonBetweenLibsTask*"
}