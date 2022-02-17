``` ini

BenchmarkDotNet=v0.13.1, OS=Windows 10.0.19044.1466 (21H2)
Intel Core i7-4770K CPU 3.50GHz (Haswell), 1 CPU, 8 logical and 4 physical cores
  [Host]             : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT
  .NET Framework 4.8 : .NET Framework 4.8 (4.8.4420.0), X64 RyuJIT

Job=.NET Framework 4.8  Runtime=.NET Framework 4.8  

```
|                          Method |     Mean |    Error |   StdDev | Ratio | RatioSD |       Gen 0 |      Gen 1 |     Gen 2 | Allocated |
|-------------------------------- |---------:|---------:|---------:|------:|--------:|------------:|-----------:|----------:|----------:|
| WriteLinq_NKZsmos_32ThreadsX160 | 141.4 ms |  2.70 ms |  3.00 ms |  1.00 |    0.00 |  11000.0000 |  2500.0000 |  500.0000 |     62 MB |
|   WriteLinq_FSH_B_32ThreadsX160 | 716.5 ms | 13.32 ms | 13.08 ms |  5.07 |    0.13 | 128000.0000 | 26000.0000 | 7000.0000 |    646 MB |
