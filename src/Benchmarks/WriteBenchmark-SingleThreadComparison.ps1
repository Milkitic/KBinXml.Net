dotnet build WriteBenchmark --configuration Release --framework net6.0
if ($LASTEXITCODE -eq 0) {
  Start-Process .\WriteBenchmark\bin\Release\net6.0\WriteBenchmark.exe -WorkingDirectory .\WriteBenchmark\bin\Release\net6.0 -NoNewWindow -Wait -ArgumentList "--filter *SingleThreadComparison*"
}