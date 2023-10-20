using System.IO;
using System.Xml;
using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using KbinXml.Net;

namespace ReadBenchmark;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.Net70)]
[SimpleJob(RuntimeMoniker.Net48)]
public class SingleThreadComparison1
{
    private byte[] _kbin;
    private byte[] _xmlBytes;
    private XDocument _linq;
    private XmlDocument _xml;
    private string _xmlStr;

    [GlobalSetup]
    public void Setup()
    {
        _kbin = KbinConverter.Write(File.ReadAllText(@"data/small.xml"), KnownEncodings.UTF8);
        //_kbin = File.ReadAllBytes(@"data\test_case.bin");
        _xmlBytes = KbinConverter.ReadXmlBytes(_kbin);
        _linq = KbinConverter.ReadXmlLinq(_kbin);
        _xml = KbinConverter.ReadXml(_kbin);
        _xmlStr = _linq.ToString();
    }

    [Benchmark(Baseline = true)]
    public object? ReadRaw()
    {
        return KbinConverter.ReadXmlBytes(_kbin);
    }

    [Benchmark]
    public object? ReadLinq()
    {
        return KbinConverter.ReadXmlLinq(_kbin);
    }

    [Benchmark]
    public object? ReadW3C()
    {
        return KbinConverter.ReadXml(_kbin);
    }
}