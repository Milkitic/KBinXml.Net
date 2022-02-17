``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.200
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                              Method |           Job |       Runtime |     Mean |   Error |  StdDev | Ratio | RatioSD |      Gen 0 |      Gen 1 |     Gen 2 |     Allocated |
|------------------------------------ |-------------- |-------------- |---------:|--------:|--------:|------:|--------:|-----------:|-----------:|----------:|--------------:|
|     WriteLinq_NKZsmos_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 146.5 ms | 0.61 ms | 0.51 ms |  1.00 |    0.00 |  2750.0000 |  1000.0000 |  250.0000 |  44,672,080 B |
| WriteLinq_ItsNovaHere_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 456.3 ms | 4.11 ms | 3.64 ms |  3.12 |    0.03 | 12000.0000 |  2000.0000 |         - | 222,915,856 B |
|       WriteLinq_FSH_B_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 626.4 ms | 8.67 ms | 8.11 ms |  4.28 |    0.05 | 35000.0000 | 14000.0000 | 4000.0000 | 592,696,656 B |
|                                     |               |               |          |         |         |       |         |            |            |           |               |
|     WriteLinq_NKZsmos_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |          - |          - |         - |             - |
|       WriteLinq_FSH_B_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |          - |          - |         - |             - |
| WriteLinq_ItsNovaHere_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |          - |          - |         - |             - |

Benchmarks with issues:
  MultiThreadComparisonBetweenLibsTask.WriteLinq_NKZsmos_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparisonBetweenLibsTask.WriteLinq_FSH_B_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparisonBetweenLibsTask.WriteLinq_ItsNovaHere_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
