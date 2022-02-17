dotnet build WriteBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process dotnet -WorkingDirectory ./WriteBenchmark/bin/Release/net6.0 -NoNewWindow -Wait -ArgumentList "WriteBenchmark.dll --filter *MultiThreadComparison1*"
}