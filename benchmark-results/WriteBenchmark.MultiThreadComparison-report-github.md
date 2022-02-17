``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.200
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|                    Method |           Job |       Runtime |     Mean |   Error |  StdDev | Ratio | RatioSD |     Gen 0 |     Gen 1 |    Gen 2 |    Allocated |
|-------------------------- |-------------- |-------------- |---------:|--------:|--------:|------:|--------:|----------:|----------:|---------:|-------------:|
|   WriteLinq_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 145.7 ms | 0.98 ms | 0.87 ms |  0.91 |    0.01 | 2750.0000 | 1000.0000 | 250.0000 | 44,672,016 B |
|    WriteW3C_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 157.4 ms | 2.08 ms | 1.84 ms |  0.99 |    0.01 | 2333.3333 |  666.6667 |        - | 43,693,739 B |
|    WriteRaw_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 159.8 ms | 1.32 ms | 1.17 ms |  1.00 |    0.00 | 3666.6667 |  666.6667 |        - | 68,033,171 B |
| WriteRawStr_32ThreadsX160 |      .NET 6.0 |      .NET 6.0 | 162.7 ms | 1.46 ms | 1.37 ms |  1.02 |    0.01 | 3333.3333 |  666.6667 |        - | 65,307,528 B |
|                           |               |               |          |         |         |       |         |           |           |          |              |
|    WriteRaw_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |         - |         - |        - |            - |
| WriteRawStr_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |         - |         - |        - |            - |
|   WriteLinq_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |         - |         - |        - |            - |
|    WriteW3C_32ThreadsX160 | .NET Core 3.1 | .NET Core 3.1 |       NA |      NA |      NA |     ? |       ? |         - |         - |        - |            - |

Benchmarks with issues:
  MultiThreadComparison1.WriteRaw_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparison1.WriteRawStr_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparison1.WriteLinq_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
  MultiThreadComparison1.WriteW3C_32ThreadsX160: .NET Core 3.1(Runtime=.NET Core 3.1)
