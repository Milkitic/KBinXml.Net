``` ini

BenchmarkDotNet=v0.13.1, OS=ubuntu 20.04
Intel Xeon Platinum 8272CL CPU 2.60GHz, 1 CPU, 2 logical and 2 physical cores
.NET SDK=6.0.200
  [Host]   : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT
  .NET 6.0 : .NET 6.0.2 (6.0.222.6406), X64 RyuJIT


```
|   Method |           Job |       Runtime |     Mean |     Error |    StdDev | Ratio | RatioSD |   Gen 0 |   Gen 1 |   Gen 2 | Allocated |
|--------- |-------------- |-------------- |---------:|----------:|----------:|------:|--------:|--------:|--------:|--------:|----------:|
| ReadLinq |      .NET 6.0 |      .NET 6.0 | 1.542 ms | 0.0049 ms | 0.0046 ms |  0.95 |    0.00 | 27.3438 | 13.6719 |       - | 519,545 B |
|  ReadRaw |      .NET 6.0 |      .NET 6.0 | 1.619 ms | 0.0058 ms | 0.0052 ms |  1.00 |    0.00 | 58.5938 | 58.5938 | 58.5938 | 591,081 B |
|  ReadW3C |      .NET 6.0 |      .NET 6.0 | 2.338 ms | 0.0154 ms | 0.0144 ms |  1.44 |    0.01 | 35.1563 | 15.6250 |       - | 683,011 B |
|          |               |               |          |           |           |       |         |         |         |         |           |
|  ReadRaw | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |       - |       - |       - |         - |
| ReadLinq | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |       - |       - |       - |         - |
|  ReadW3C | .NET Core 3.1 | .NET Core 3.1 |       NA |        NA |        NA |     ? |       ? |       - |       - |       - |         - |

Benchmarks with issues:
  SingleThreadComparison1.ReadRaw: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparison1.ReadLinq: .NET Core 3.1(Runtime=.NET Core 3.1)
  SingleThreadComparison1.ReadW3C: .NET Core 3.1(Runtime=.NET Core 3.1)
