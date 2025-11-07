using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using KbinXml.Net;
using Microsoft.IO;
using DataWriter = KbinXml.Net.Internal.Writers.DataWriter;

namespace UnitBenchmarks;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net10_0)]
//[SimpleJob(RuntimeMoniker.Net48)]
public class DataWriterU8U32Benchmark
{
    private RecyclableMemoryStream _stream = null!;
    private byte[] _array = null!;

    [GlobalSetup]
    public void Setup()
    {
        _stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("", ushort.MaxValue);
        _array = new byte[512];
        new Random(1996).NextBytes(_array);
    }

    [Benchmark(Baseline = true)]
    public object? OldWriter()
    {
        _stream.SetLength(0);
        using var writer = new Legacy.DataWriter(Encoding.UTF8, _stream);
        for (var i = 0; i < _array.Length - 7; i += 8)
        {
            writer.WriteU8(_array[i]);
            writer.WriteU32(_array[i + 1]);
            writer.WriteU8(_array[i + 2]);
            writer.WriteU8(_array[i + 3]);
            writer.WriteU8(_array[i + 4]);
            writer.WriteU8(_array[i + 5]);
            writer.WriteU8(_array[i + 6]);
            writer.WriteU32(_array[i + 7]);
        }

        return _stream;
    }

    [Benchmark]
    public object? NewWriter()
    {
        _stream.SetLength(0);
        using var writer = new DataWriter(Encoding.UTF8, _stream);
        for (var i = 0; i < _array.Length - 7; i += 8)
        {
            writer.WriteU8(_array[i]);
            writer.WriteU32(_array[i + 1]);
            writer.WriteU8(_array[i + 2]);
            writer.WriteU8(_array[i + 3]);
            writer.WriteU8(_array[i + 4]);
            writer.WriteU8(_array[i + 5]);
            writer.WriteU8(_array[i + 6]);
            writer.WriteU32(_array[i + 7]);
        }

        return _stream;
    }
}