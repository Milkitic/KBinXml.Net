```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 10 (10.0.20348.2113) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100
  [Host]             : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  .NET 6.0           : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  .NET 7.0           : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  .NET 8.0           : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2
  .NET Framework 4.8 : .NET Framework 4.8.1 (4.8.9186.0), X64 RyuJIT VectorSize=256


```
| Method                    | Job                | Runtime            | Mean      | Error    | StdDev   | Median    | Ratio | RatioSD | Gen0       | Gen1      | Gen2     | Allocated | Alloc Ratio |
|-------------------------- |------------------- |------------------- |----------:|---------:|---------:|----------:|------:|--------:|-----------:|----------:|---------:|----------:|------------:|
| WriteLinq_32ThreadsX160   | .NET 6.0           | .NET 6.0           |  89.76 ms | 0.306 ms | 0.286 ms |  89.66 ms |  0.96 |    0.00 |  2500.0000 |  833.3333 | 333.3333 |  37.04 MB |        0.62 |
| WriteRaw_32ThreadsX160    | .NET 6.0           | .NET 6.0           |  93.89 ms | 0.306 ms | 0.271 ms |  93.88 ms |  1.00 |    0.00 |  4000.0000 | 1000.0000 | 333.3333 |  59.31 MB |        1.00 |
| WriteW3C_32ThreadsX160    | .NET 6.0           | .NET 6.0           |  94.87 ms | 0.308 ms | 0.273 ms |  94.78 ms |  1.01 |    0.00 |  2500.0000 |  833.3333 | 333.3333 |  36.11 MB |        0.61 |
| WriteRawStr_32ThreadsX160 | .NET 6.0           | .NET 6.0           |  97.08 ms | 0.271 ms | 0.240 ms |  97.08 ms |  1.03 |    0.00 |  4000.0000 | 1000.0000 | 400.0000 |  56.73 MB |        0.96 |
|                           |                    |                    |           |          |          |           |       |         |            |           |          |           |             |
| WriteLinq_32ThreadsX160   | .NET 7.0           | .NET 7.0           |  84.50 ms | 0.405 ms | 0.379 ms |  84.36 ms |  0.93 |    0.01 |  2666.6667 | 1000.0000 | 500.0000 |   36.7 MB |        0.62 |
| WriteRaw_32ThreadsX160    | .NET 7.0           | .NET 7.0           |  91.17 ms | 0.518 ms | 0.485 ms |  91.13 ms |  1.00 |    0.00 |  4000.0000 | 1333.3333 | 500.0000 |  58.96 MB |        1.00 |
| WriteW3C_32ThreadsX160    | .NET 7.0           | .NET 7.0           |  91.67 ms | 0.367 ms | 0.343 ms |  91.58 ms |  1.01 |    0.01 |  2666.6667 | 1166.6667 | 500.0000 |  35.77 MB |        0.61 |
| WriteRawStr_32ThreadsX160 | .NET 7.0           | .NET 7.0           |  93.03 ms | 0.262 ms | 0.232 ms |  92.99 ms |  1.02 |    0.01 |  4000.0000 | 1333.3333 | 500.0000 |  56.39 MB |        0.96 |
|                           |                    |                    |           |          |          |           |       |         |            |           |          |           |             |
| WriteLinq_32ThreadsX160   | .NET 8.0           | .NET 8.0           |  71.04 ms | 0.305 ms | 0.254 ms |  70.97 ms |  0.89 |    0.01 |  2333.3333 |  666.6667 | 333.3333 |   36.7 MB |        0.62 |
| WriteW3C_32ThreadsX160    | .NET 8.0           | .NET 8.0           |  75.45 ms | 0.410 ms | 0.342 ms |  75.49 ms |  0.95 |    0.01 |  2333.3333 |  666.6667 | 333.3333 |  35.77 MB |        0.61 |
| WriteRaw_32ThreadsX160    | .NET 8.0           | .NET 8.0           |  79.41 ms | 1.064 ms | 0.943 ms |  79.21 ms |  1.00 |    0.00 |  4000.0000 | 1000.0000 | 500.0000 |  58.96 MB |        1.00 |
| WriteRawStr_32ThreadsX160 | .NET 8.0           | .NET 8.0           |  84.71 ms | 1.554 ms | 2.325 ms |  83.47 ms |  1.08 |    0.04 |  3500.0000 | 1000.0000 | 500.0000 |  56.39 MB |        0.96 |
|                           |                    |                    |           |          |          |           |       |         |            |           |          |           |             |
| WriteLinq_32ThreadsX160   | .NET Framework 4.8 | .NET Framework 4.8 | 138.28 ms | 0.620 ms | 0.580 ms | 138.50 ms |  0.91 |    0.00 |  8750.0000 | 1750.0000 | 500.0000 |  50.17 MB |        0.68 |
| WriteW3C_32ThreadsX160    | .NET Framework 4.8 | .NET Framework 4.8 | 143.09 ms | 0.699 ms | 0.654 ms | 143.32 ms |  0.95 |    0.00 |  8500.0000 | 1750.0000 | 500.0000 |  49.42 MB |        0.67 |
| WriteRaw_32ThreadsX160    | .NET Framework 4.8 | .NET Framework 4.8 | 151.19 ms | 0.362 ms | 0.339 ms | 151.22 ms |  1.00 |    0.00 | 12000.0000 | 2000.0000 | 750.0000 |   73.3 MB |        1.00 |
| WriteRawStr_32ThreadsX160 | .NET Framework 4.8 | .NET Framework 4.8 | 154.40 ms | 0.487 ms | 0.407 ms | 154.27 ms |  1.02 |    0.00 | 12000.0000 | 2250.0000 | 750.0000 |  70.48 MB |        0.96 |
