using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using KbinXml.Net;
using KbinXml.Net.Internal.Writers;
using Microsoft.IO;

namespace UnitBenchmarks;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net10_0)]
//[SimpleJob(RuntimeMoniker.Net48)]
public class DataWriterBinaryBenchmark
{
    private RecyclableMemoryStream _stream = null!;
    private byte[] _array = null!;
    private Random _rng = null!; // 需要一个 Random 实例

    public List<string> PreGeneratedHexStrings { get; } = new List<string>();

    private const int TotalStringsToGenerate = 20; // 准备 20 条测试数据
    private const int MaxBytesForString = 1024;    // 单个字符串最大由 1024 字节转换而来
    private const int ByteArraySize = 10240;       // 确保源数组足够大

    [GlobalSetup]
    public void Setup()
    {
        _stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("", ushort.MaxValue);
        _array = new byte[ByteArraySize];

        _rng = new Random(1996);
        _rng.NextBytes(_array);

        for (int i = 0; i < TotalStringsToGenerate; i++)
        {
            int length = _rng.Next(1, MaxBytesForString + 1);
            int offset = _rng.Next(0, _array.Length - length);

            var dataSlice = new ReadOnlySpan<byte>(_array, offset, length);

            var sb = new StringBuilder(dataSlice.Length * 2);
            foreach (byte b in dataSlice)
            {
                sb.Append(b.ToString("X2"));
            }

            string hexString = sb.ToString();

            PreGeneratedHexStrings.Add(hexString);
        }
    }

    [Benchmark(Baseline = true)]
    public object? OldWriter()
    {
        _stream.SetLength(0);
        using var writer = new Legacy.DataWriterV1(Encoding.UTF8, _stream);
        for (var i = 0; i < PreGeneratedHexStrings.Count; i++)
        {
            var b = PreGeneratedHexStrings[i];
            writer.WriteBinary(b.AsSpan());
        }

        return _stream;
    }

    [Benchmark()]
    public object? NewWriter()
    {
        _stream.SetLength(0);
        using var writer = new DataWriter(Encoding.UTF8, _stream);
        for (var i = 0; i < PreGeneratedHexStrings.Count; i++)
        {
            var b = PreGeneratedHexStrings[i];
            writer.WriteBinary(b.AsSpan());
        }

        return _stream;
    }
}