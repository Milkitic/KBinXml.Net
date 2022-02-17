``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.200
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|      Method |           Job |       Runtime |     Mean |     Error |    StdDev | Ratio | RatioSD |   Gen 0 |  Gen 1 | Allocated |
|------------ |-------------- |-------------- |---------:|----------:|----------:|------:|--------:|--------:|-------:|----------:|
|   WriteLinq |      .NET 6.0 |      .NET 6.0 | 1.765 ms | 0.0033 ms | 0.0029 ms |  0.92 |    0.00 | 13.6719 | 3.9063 | 279,114 B |
|    WriteRaw |      .NET 6.0 |      .NET 6.0 | 1.917 ms | 0.0050 ms | 0.0047 ms |  1.00 |    0.00 | 21.4844 | 5.8594 | 425,129 B |
|    WriteW3C |      .NET 6.0 |      .NET 6.0 | 1.946 ms | 0.0125 ms | 0.0117 ms |  1.02 |    0.01 | 11.7188 |      - | 273,003 B |
| WriteRawStr |      .NET 6.0 |      .NET 6.0 | 1.995 ms | 0.0049 ms | 0.0045 ms |  1.04 |    0.00 | 19.5313 | 3.9063 | 408,091 B |
|             |               |               |          |           |           |       |         |         |        |           |
|    WriteRaw | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |       - |      - |         - |
| WriteRawStr | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |       - |      - |         - |
|   WriteLinq | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |       - |      - |         - |
|    WriteW3C | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |       - |      - |         - |

Benchmarks with issues:
  SingleThreadComparison1.WriteRaw: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparison1.WriteRawStr: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparison1.WriteLinq: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparison1.WriteW3C: .NET Core 3.1(Runtime=.NET Core 3.1)
