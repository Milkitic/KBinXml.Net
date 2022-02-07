dotnet build ReadBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process ./ReadBenchmark/bin/Release/net6.0/ReadBenchmark.exe -WorkingDirectory ./ReadBenchmark/bin/Release/net6.0 -NoNewWindow -Wait -ArgumentList "--filter *MultiThreadComparison*"
}