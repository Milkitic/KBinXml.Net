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
[SimpleJob(RuntimeMoniker.Net48)]
public class DataWriterBenchmark
{
    private RecyclableMemoryStream _stream;
    private const string CardId = "E004E0D1423A4EE2";
    private const string Text = "Hello";

    [GlobalSetup]
    public void Setup()
    {
        _stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("", ushort.MaxValue);
    }

    [Benchmark(Baseline = true)]
    public object? Datawriter1()
    {
        _stream.SetLength(0);
        using var writer = new Legacy.DataWriter(Encoding.UTF8, _stream);

        writer.WriteS8(1); // 1 byte
        writer.WriteBinary(CardId.AsSpan());
        writer.WriteS16(2); // 2 bytes
        writer.WriteS8(3); // 1 byte
        writer.WriteString(Text.AsSpan());
        writer.WriteS16(1996); // 2 bytes
        writer.WriteS32(4); // 4 bytes
        writer.WriteU8(240); // 1 byte
        return _stream;
    }

    [Benchmark]
    public object? DataWriter2()
    {
        _stream.SetLength(0);
        using var writer = new DataWriter(Encoding.UTF8, _stream);

        writer.WriteS8(1); // 1 byte
        writer.WriteBinary(CardId.AsSpan());
        writer.WriteS16(2); // 2 bytes
        writer.WriteS8(3); // 1 byte
        writer.WriteString(Text.AsSpan());
        writer.WriteS16(1996); // 2 bytes
        writer.WriteS32(4); // 4 bytes
        writer.WriteU8(240); // 1 byte
        return _stream;
    }
}