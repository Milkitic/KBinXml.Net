```

BenchmarkDotNet v0.13.9+228a464e8be6c580ad9408e98f18813f6407fb5a, Windows 10 (10.0.20348.2113) (Hyper-V)
AMD EPYC 7763, 1 CPU, 4 logical and 2 physical cores
.NET SDK 8.0.100
  [Host]   : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  .NET 6.0 : .NET 6.0.25 (6.0.2523.51912), X64 RyuJIT AVX2
  .NET 7.0 : .NET 7.0.14 (7.0.1423.51910), X64 RyuJIT AVX2
  .NET 8.0 : .NET 8.0.0 (8.0.23.53103), X64 RyuJIT AVX2


```
| Method                              | Job      | Runtime  | Mean      | Error    | StdDev    | Ratio | RatioSD | Gen0       | Gen1       | Gen2      | Allocated | Alloc Ratio |
|------------------------------------ |--------- |--------- |----------:|---------:|----------:|------:|--------:|-----------:|-----------:|----------:|----------:|------------:|
| WriteLinq_NKZsmos_32ThreadsX160     | .NET 6.0 | .NET 6.0 |  90.28 ms | 0.374 ms |  0.331 ms |  1.00 |    0.00 |  2500.0000 |   833.3333 |  333.3333 |  37.04 MB |        1.00 |
| WriteLinq_ItsNovaHere_32ThreadsX160 | .NET 6.0 | .NET 6.0 | 252.99 ms | 0.757 ms |  0.671 ms |  2.80 |    0.01 | 14000.0000 |  2000.0000 |  500.0000 | 212.59 MB |        5.74 |
| WriteLinq_FSH_B_32ThreadsX160       | .NET 6.0 | .NET 6.0 | 439.16 ms | 8.703 ms |  8.141 ms |  4.87 |    0.09 | 39000.0000 | 22000.0000 | 9000.0000 | 565.89 MB |       15.28 |
|                                     |          |          |           |          |           |       |         |            |            |           |           |             |
| WriteLinq_NKZsmos_32ThreadsX160     | .NET 7.0 | .NET 7.0 |  84.53 ms | 0.437 ms |  0.409 ms |  1.00 |    0.00 |  2666.6667 |  1000.0000 |  500.0000 |   36.7 MB |        1.00 |
| WriteLinq_ItsNovaHere_32ThreadsX160 | .NET 7.0 | .NET 7.0 | 246.76 ms | 0.835 ms |  0.740 ms |  2.92 |    0.02 | 14000.0000 |  2000.0000 |  500.0000 | 212.59 MB |        5.79 |
| WriteLinq_FSH_B_32ThreadsX160       | .NET 7.0 | .NET 7.0 | 409.19 ms | 7.659 ms |  7.865 ms |  4.84 |    0.09 | 39000.0000 | 33000.0000 | 9000.0000 |  565.9 MB |       15.42 |
|                                     |          |          |           |          |           |       |         |            |            |           |           |             |
| WriteLinq_NKZsmos_32ThreadsX160     | .NET 8.0 | .NET 8.0 |  70.93 ms | 0.585 ms |  0.519 ms |  1.00 |    0.00 |  2333.3333 |   666.6667 |  333.3333 |   36.7 MB |        1.00 |
| WriteLinq_ItsNovaHere_32ThreadsX160 | .NET 8.0 | .NET 8.0 | 203.71 ms | 0.658 ms |  0.583 ms |  2.87 |    0.02 | 13000.0000 |  2000.0000 |         - | 212.59 MB |        5.79 |
| WriteLinq_FSH_B_32ThreadsX160       | .NET 8.0 | .NET 8.0 | 385.23 ms | 7.665 ms | 11.934 ms |  5.45 |    0.18 | 39000.0000 | 36000.0000 | 9000.0000 | 565.88 MB |       15.42 |
