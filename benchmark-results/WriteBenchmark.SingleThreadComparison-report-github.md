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
| Method      | Job                | Runtime            | Mean       | Error   | StdDev  | Ratio | Gen0    | Gen1    | Allocated | Alloc Ratio |
|------------ |------------------- |------------------- |-----------:|--------:|--------:|------:|--------:|--------:|----------:|------------:|
| WriteLinq   | .NET 6.0           | .NET 6.0           | 1,187.8 μs | 2.15 μs | 2.01 μs |  0.95 | 13.6719 |  1.9531 | 236.99 KB |        0.62 |
| WriteRaw    | .NET 6.0           | .NET 6.0           | 1,256.1 μs | 1.74 μs | 1.55 μs |  1.00 | 21.4844 |  3.9063 | 379.47 KB |        1.00 |
| WriteRawStr | .NET 6.0           | .NET 6.0           | 1,297.6 μs | 2.10 μs | 1.86 μs |  1.03 | 21.4844 |  3.9063 | 363.02 KB |        0.96 |
| WriteW3C    | .NET 6.0           | .NET 6.0           | 1,481.2 μs | 3.78 μs | 3.54 μs |  1.18 | 13.6719 |  1.9531 | 231.03 KB |        0.61 |
|             |                    |                    |            |         |         |       |         |         |           |             |
| WriteLinq   | .NET 7.0           | .NET 7.0           | 1,112.3 μs | 2.74 μs | 2.56 μs |  0.94 | 13.6719 |  1.9531 | 234.81 KB |        0.62 |
| WriteRaw    | .NET 7.0           | .NET 7.0           | 1,182.7 μs | 2.35 μs | 2.08 μs |  1.00 | 21.4844 |  3.9063 | 377.28 KB |        1.00 |
| WriteRawStr | .NET 7.0           | .NET 7.0           | 1,217.1 μs | 1.76 μs | 1.65 μs |  1.03 | 21.4844 |  3.9063 | 360.83 KB |        0.96 |
| WriteW3C    | .NET 7.0           | .NET 7.0           | 1,263.0 μs | 4.52 μs | 4.23 μs |  1.07 | 13.6719 |       - | 228.84 KB |        0.61 |
|             |                    |                    |            |         |         |       |         |         |           |             |
| WriteLinq   | .NET 8.0           | .NET 8.0           |   959.7 μs | 1.98 μs | 1.85 μs |  0.91 | 13.6719 |  2.9297 |  234.8 KB |        0.62 |
| WriteW3C    | .NET 8.0           | .NET 8.0           |   996.5 μs | 1.15 μs | 1.07 μs |  0.95 | 13.6719 |       - | 228.84 KB |        0.61 |
| WriteRaw    | .NET 8.0           | .NET 8.0           | 1,052.5 μs | 1.76 μs | 1.47 μs |  1.00 | 19.5313 |  3.9063 | 377.21 KB |        1.00 |
| WriteRawStr | .NET 8.0           | .NET 8.0           | 1,137.4 μs | 2.07 μs | 1.84 μs |  1.08 | 19.5313 |  3.9063 | 360.83 KB |        0.96 |
|             |                    |                    |            |         |         |       |         |         |           |             |
| WriteLinq   | .NET Framework 4.8 | .NET Framework 4.8 | 1,858.0 μs | 4.21 μs | 3.29 μs |  0.92 | 44.9219 |  7.8125 | 288.09 KB |        0.65 |
| WriteW3C    | .NET Framework 4.8 | .NET Framework 4.8 | 1,989.1 μs | 5.18 μs | 4.85 μs |  0.98 | 42.9688 |  7.8125 | 282.06 KB |        0.64 |
| WriteRaw    | .NET Framework 4.8 | .NET Framework 4.8 | 2,023.3 μs | 3.10 μs | 2.42 μs |  1.00 | 70.3125 | 15.6250 | 443.12 KB |        1.00 |
| WriteRawStr | .NET Framework 4.8 | .NET Framework 4.8 | 2,072.2 μs | 5.81 μs | 5.43 μs |  1.02 | 66.4063 | 15.6250 | 426.47 KB |        0.96 |
