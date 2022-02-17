``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.200
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                Method |           Job |       Runtime |     Mean |     Error |    StdDev | Ratio | RatioSD |    Gen 0 |   Gen 1 |   Gen 2 |   Allocated |
|---------------------- |-------------- |-------------- |---------:|----------:|----------:|------:|--------:|---------:|--------:|--------:|------------:|
|     WriteLinq_NKZsmos |      .NET 6.0 |      .NET 6.0 | 1.769 ms | 0.0025 ms | 0.0024 ms |  1.00 |    0.00 |  13.6719 |  3.9063 |       - |   279,113 B |
| WriteLinq_ItsNovaHere |      .NET 6.0 |      .NET 6.0 | 4.620 ms | 0.0158 ms | 0.0148 ms |  2.61 |    0.01 |  70.3125 | 15.6250 |       - | 1,393,126 B |
|       WriteLinq_FSH_B |      .NET 6.0 |      .NET 6.0 | 6.613 ms | 0.0577 ms | 0.0540 ms |  3.74 |    0.03 | 179.6875 | 93.7500 | 54.6875 | 3,704,210 B |
|                       |               |               |          |           |           |       |         |          |         |         |             |
|     WriteLinq_NKZsmos | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |        - |       - |       - |           - |
|       WriteLinq_FSH_B | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |        - |       - |       - |           - |
| WriteLinq_ItsNovaHere | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |        - |       - |       - |           - |

Benchmarks with issues:
  SingleThreadComparisonBetweenLibsTask.WriteLinq_NKZsmos: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparisonBetweenLibsTask.WriteLinq_FSH_B: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparisonBetweenLibsTask.WriteLinq_ItsNovaHere: .NET Core 3.1(Runtime=.NET Core 3.1)
