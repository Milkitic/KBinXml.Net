``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.200
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|               Method |           Job |       Runtime |     Mean |     Error |    StdDev | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 |   Allocated |
|--------------------- |-------------- |-------------- |---------:|----------:|----------:|------:|--------:|---------:|--------:|--------:|------------:|
|     ReadLinq_NKZsmos |      .NET 6.0 |      .NET 6.0 | 1.542 ms | 0.0058 ms | 0.0054 ms |  1.00 |    0.00 |  27.3438 | 13.6719 |       - |   519,545 B |
| ReadLinq_ItsNovaHere |      .NET 6.0 |      .NET 6.0 | 4.418 ms | 0.0180 ms | 0.0160 ms |  2.86 |    0.02 | 109.3750 | 54.6875 |       - | 2,184,266 B |
|       ReadLinq_FSH_B |      .NET 6.0 |      .NET 6.0 | 5.764 ms | 0.0674 ms | 0.0630 ms |  3.74 |    0.05 | 148.4375 | 93.7500 | 46.8750 | 2,449,438 B |
|                      |               |               |          |           |           |       |         |          |         |         |             |
|     ReadLinq_NKZsmos | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |        - |       - |       - |           - |
|       ReadLinq_FSH_B | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |        - |       - |       - |           - |
| ReadLinq_ItsNovaHere | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |        - |       - |       - |           - |

Benchmarks with issues:
  SingleThreadComparisonBetweenLibsTask.ReadLinq_NKZsmos: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparisonBetweenLibsTask.ReadLinq_FSH_B: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparisonBetweenLibsTask.ReadLinq_ItsNovaHere: .NET Core 3.1(Runtime=.NET Core 3.1)
