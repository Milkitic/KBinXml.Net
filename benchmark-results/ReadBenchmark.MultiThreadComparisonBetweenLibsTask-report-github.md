``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.200
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                             Method |           Job |       Runtime |     Mean |   Error |  StdDev | Ratio | RatioSD |      Gen 0 |     Gen 1 |     Allocated |
|----------------------------------- |-------------- |-------------- |---------:|--------:|--------:|------:|--------:|-----------:|----------:|--------------:|
|     ReadLinq_NKZsmos_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 320.3 ms | 4.39 ms | 3.89 ms |  1.00 |    0.00 |  4000.0000 | 2000.0000 |  83,141,688 B |
| ReadLinq_ItsNovaHere_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 690.0 ms | 5.43 ms | 5.08 ms |  2.15 |    0.02 | 18000.0000 | 5000.0000 | 349,552,056 B |
|       ReadLinq_FSH_B_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 693.8 ms | 6.41 ms | 5.69 ms |  2.17 |    0.03 | 20000.0000 | 7000.0000 | 391,911,528 B |
|                                    |               |               |          |         |         |       |         |            |           |               |
|     ReadLinq_NKZsmos_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |          - |         - |             - |
|       ReadLinq_FSH_B_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |          - |         - |             - |
| ReadLinq_ItsNovaHere_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |          - |         - |             - |

Benchmarks with issues:
  MultiThreadComparisonBetweenLibsTask.ReadLinq_NKZsmos_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparisonBetweenLibsTask.ReadLinq_FSH_B_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparisonBetweenLibsTask.ReadLinq_ItsNovaHere_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
