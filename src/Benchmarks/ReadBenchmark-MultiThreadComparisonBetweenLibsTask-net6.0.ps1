dotnet build ReadBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process dotnet -WorkingDirectory ./ReadBenchmark/bin/Release/net6.0 -NoNewWindow -Wait -ArgumentList "ReadBenchmark.dll --filter *MultiThreadComparisonBetweenLibsTask*"
}