using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using KbinXml.Net;

namespace RepairBenchmark;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net48)]
public class RepairBenchmarkTask
{
    private byte[] _kbin = null!;
    private string _xmlStr = null!;

    [GlobalSetup]
    public void Setup()
    {
        _kbin = KbinConverter.Write(File.ReadAllText(@"data/test_case.xml"), KnownEncodings.ShiftJIS);
        var linq = KbinConverter.ReadXmlLinq(_kbin);
        _xmlStr = linq.ToString();
    }

    [Benchmark(Baseline = true)]
    public object? Read()
    {
        return KbinConverter.ReadXmlBytes(_kbin);
    }

    [Benchmark]
    public object? ReadWithRepairShortPrefix()
    {
        return KbinConverter.ReadXmlBytes(_kbin, new ReadOptions { RepairedPrefix = "PREFIX_" });
    }

    [Benchmark]
    public object? ReadWithRepairLongPrefix()
    {
        return KbinConverter.ReadXmlBytes(_kbin, new ReadOptions { RepairedPrefix = "KONMAI_BINARY_PREFIX_FIX_" });
    }

    [Benchmark]
    public object? Write()
    {
        return KbinConverter.Write(_xmlStr, KnownEncodings.ShiftJIS);
    }

    [Benchmark]
    public object? WriteWithRepairShortPrefix()
    {
        return KbinConverter.Write(_xmlStr, KnownEncodings.ShiftJIS, new WriteOptions { RepairedPrefix = "PREFIX_" });
    }

    [Benchmark]
    public object? WriteWithRepairLongPrefix()
    {
        return KbinConverter.Write(_xmlStr, KnownEncodings.ShiftJIS, new WriteOptions { RepairedPrefix = "KONMAI_BINARY_PREFIX_FIX_" });
    }
}