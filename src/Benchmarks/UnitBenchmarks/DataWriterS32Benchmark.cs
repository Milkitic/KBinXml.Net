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
public class DataWriterS32Benchmark
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
        using var writer = new Legacy.DataWriterV1(Encoding.UTF8, _stream);
        for (var i = 0; i < _array.Length; i++)
        {
            var b = _array[i];
            writer.WriteS32(b);
            _stream.SetLength(_stream.Length);
        }

        return _stream;
    }

    [Benchmark]
    public object? OldWriterWithGap()
    {
        _stream.SetLength(0);
        using var writer = new Legacy.DataWriterV1(Encoding.UTF8, _stream);
        for (var i = 0; i < _array.Length; i++)
        {
            var b = _array[i];
            writer.WriteS32(b);
            _stream.SetLength(_stream.Length - 4);
        }

        return _stream;
    }

    [Benchmark]
    public object? NewWriter()
    {
        _stream.SetLength(0);
        using var writer = new DataWriter(Encoding.UTF8, _stream);
        for (var i = 0; i < _array.Length; i++)
        {
            var b = _array[i];
            writer.WriteS32(b);
            _stream.SetLength(_stream.Length);
        }

        return _stream;
    }

    [Benchmark]
    public object? NewWriterWithGap()
    {
        _stream.SetLength(0);
        using var writer = new DataWriter(Encoding.UTF8, _stream);
        for (var i = 0; i < _array.Length; i++)
        {
            var b = _array[i];
            writer.WriteS32(b);
            _stream.SetLength(_stream.Length - 4);
        }

        return _stream;
    }
}