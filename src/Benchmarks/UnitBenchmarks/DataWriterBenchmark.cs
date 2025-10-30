using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using KbinXml.Net.Internal.Writers;

namespace UnitBenchmarks;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net80, baseline: true)]
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net48)]
public class DataWriterBenchmark
{

    [GlobalSetup]
    public void Setup()
    {

    }

    [Benchmark]
    public object? Alignment_Complex_Sequence_WithStringBin()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS8(1); // 1 byte
        writer.WriteBinary("E004E0D1423A4EE2".AsSpan());
        writer.WriteS16(2); // 2 bytes
        writer.WriteS8(3); // 1 byte
        writer.WriteString("Hello".AsSpan());
        writer.WriteS16(1996); // 2 bytes
        writer.WriteS32(4); // 4 bytes
        writer.WriteU8(240); // 1 byte
        return writer.Stream.GetBuffer();
    }
}