Push-Location ./ReadBenchmark
try {
    dotnet build --configuration Release --framework "net9.0" /p:DefineConstants=kbin1_1
    dotnet run --configuration Release --framework "net9.0" --filter "*MultiThreadComparison1*" --no-build
}
finally {
    Pop-Location
}