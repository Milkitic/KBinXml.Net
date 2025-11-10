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
[SimpleJob(RuntimeMoniker.Net48)]
public class DataWriterBenchmark2
{
    private RecyclableMemoryStream _stream = null!;
    private List<Memory<byte>> _chunks = null!;
    private byte[] _bytes = null!;

    [GlobalSetup]
    public void Setup()
    {
        _stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("", ushort.MaxValue);
        _bytes = new byte[12356];
        var random = new Random(1996);
        random.NextBytes(_bytes);
        var list = GetChunks(_bytes, random);

        _chunks = list;
    }

    [Benchmark(Baseline = true)]
    public object? Datawriter1()
    {
        _stream.SetLength(0);
        using var writer = new Legacy.DataWriterV1(Encoding.UTF8, _stream);

        foreach (var memory in _chunks)
        {
            writer.WriteBytes(memory.Span);
        }

        return _stream;
    }

    [Benchmark]
    public object? DataWriter2()
    {
        _stream.SetLength(0);
        using var writer = new DataWriter(Encoding.UTF8, _stream);
        foreach (var memory in _chunks)
        {
            writer.WriteBytes(memory.Span);
        }

        return _stream;
    }

    private static List<Memory<byte>> GetChunks(byte[] bytes, Random random)
    {
        var list = new List<Memory<byte>>();
        var offset = 0;
        while (offset < bytes.Length)
        {
            var remaining = bytes.Length - offset;
            var i = random.Next(0, 10);
            int len;

            if (i <= 2)
            {
                // 取1byte
                len = 1;
            }
            else if (i <= 4)
            {
                // 取2bytes
                len = 2;
            }
            else
            {
                // 取随机的4的倍数bytes
                if (remaining >= 4)
                {
                    var maxMultiple = remaining / 4; // 可取的最大4的倍数
                    len = 4 * random.Next(1, maxMultiple + 1);
                }
                else
                {
                    // 剩余不足4时，退化为1或2字节以保证能取完
                    len = remaining >= 2 ? 2 : 1;
                }
            }

            if (len > remaining)
            {
                len = remaining;
            }

            list.Add(bytes.AsMemory(offset, len));
            offset += len;
        }

        return list;
    }
}