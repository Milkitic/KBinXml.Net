dotnet build ReadBenchmark --configuration Release --framework net48
if ($LASTEXITCODE -eq 0) {
  Start-Process .\ReadBenchmark\bin\Release\net48\ReadBenchmark.exe -WorkingDirectory .\ReadBenchmark\bin\Release\net48 -NoNewWindow -Wait -ArgumentList "--filter *SingleThreadComparisonBetweenLibsTask*"
}