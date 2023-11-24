```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 10 (10.0.20348.2113) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  .NET 7.0 : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


```
| Method                             | Job      | Runtime  | Mean     | Error   | StdDev  | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|----------------------------------- |--------- |--------- |---------:|--------:|--------:|------:|--------:|-----------:|-----------:|----------:|----------:|------------:|
| ReadLinq_NKZsmos_32ThreadsX160     | .NET 6.0 | .NET 6.0 | 227.8 ms | 4.51 ms | 8.14 ms |  1.00 |    0.00 |  6000.0000 |  3333.3333 | 1000.0000 |  79.31 MB |        1.00 |
| ReadLinq_ItsNovaHere_32ThreadsX160 | .NET 6.0 | .NET 6.0 | 439.2 ms | 7.05 ms | 6.59 ms |  1.93 |    0.05 | 22000.0000 |  6000.0000 | 2000.0000 | 333.49 MB |        4.21 |
| ReadLinq_FSH_B_32ThreadsX160       | .NET 6.0 | .NET 6.0 | 492.8 ms | 6.24 ms | 5.83 ms |  2.17 |    0.06 | 25000.0000 | 10000.0000 | 3000.0000 | 373.77 MB |        4.71 |
|                                    |          |          |          |         |         |       |         |            |            |           |           |             |
| ReadLinq_NKZsmos_32ThreadsX160     | .NET 7.0 | .NET 7.0 | 227.7 ms | 4.45 ms | 4.37 ms |  1.00 |    0.00 |  6666.6667 |  6333.3333 | 1666.6667 |  79.31 MB |        1.00 |
| ReadLinq_ItsNovaHere_32ThreadsX160 | .NET 7.0 | .NET 7.0 | 433.6 ms | 5.52 ms | 4.90 ms |  1.91 |    0.04 | 21000.0000 |  8000.0000 | 2000.0000 | 333.49 MB |        4.20 |
| ReadLinq_FSH_B_32ThreadsX160       | .NET 7.0 | .NET 7.0 | 456.8 ms | 7.08 ms | 6.63 ms |  2.01 |    0.05 | 26000.0000 | 14000.0000 | 4000.0000 |  373.7 MB |        4.71 |
|                                    |          |          |          |         |         |       |         |            |            |           |           |             |
| ReadLinq_NKZsmos_32ThreadsX160     | .NET 8.0 | .NET 8.0 | 197.6 ms | 3.92 ms | 6.44 ms |  1.00 |    0.00 |  6000.0000 |  5500.0000 | 1500.0000 |  77.61 MB |        1.00 |
| ReadLinq_ItsNovaHere_32ThreadsX160 | .NET 8.0 | .NET 8.0 | 352.8 ms | 4.66 ms | 4.36 ms |  1.77 |    0.07 | 21000.0000 |  6000.0000 | 2000.0000 | 331.14 MB |        4.27 |
| ReadLinq_FSH_B_32ThreadsX160       | .NET 8.0 | .NET 8.0 | 405.8 ms | 7.40 ms | 6.56 ms |  2.04 |    0.09 | 25000.0000 | 12000.0000 | 3000.0000 | 371.99 MB |        4.79 |
