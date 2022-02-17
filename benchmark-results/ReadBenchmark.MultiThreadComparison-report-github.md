``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.200
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                 Method |           Job |       Runtime |     Mean |   Error |  StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |     Allocated |
|----------------------- |-------------- |-------------- |---------:|--------:|--------:|------:|--------:|----------:|----------:|---------:|--------------:|
|  ReadRaw_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 132.1 ms | 1.66 ms | 1.47 ms |  1.00 |    0.00 | 4250.0000 | 1250.0000 | 750.0000 |  94,580,122 B |
| ReadLinq_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 321.4 ms | 4.79 ms | 4.25 ms |  2.43 |    0.04 | 4000.0000 | 2000.0000 |        - |  83,140,240 B |
|  ReadW3C_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 372.1 ms | 5.33 ms | 4.73 ms |  2.82 |    0.05 | 5000.0000 | 2000.0000 |        - | 109,297,728 B |
|                        |               |               |          |         |         |       |         |           |           |          |               |
|  ReadRaw_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |         - |         - |        - |             - |
| ReadLinq_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |         - |         - |        - |             - |
|  ReadW3C_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |         - |         - |        - |             - |

Benchmarks with issues:
  MultiThreadComparison1.ReadRaw_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparison1.ReadLinq_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparison1.ReadW3C_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
