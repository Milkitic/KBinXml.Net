using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using KBinXML;
using KbinXml.Net;
using kbinxmlcs;
#if NETCOREAPP
#endif

namespace ReadBenchmark;

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[MemoryDiagnoser]
#if !NETCOREAPP
[SimpleJob(RuntimeMoniker.Net48)]
#else
[SimpleJob(RuntimeMoniker.Net60)]
[SimpleJob(RuntimeMoniker.NetCoreApp31)]
#endif
public class MultiThreadComparisonBetweenLibsTask
{
    private byte[] _kbin;
    private byte[] _xmlBytes;
    private XDocument _linq;
    private XmlDocument _xml;
    private string _xmlStr;

    [GlobalSetup]
    public void Setup()
    {
        _kbin = KbinConverter.Write(File.ReadAllText(@"data\small.xml"), KnownEncodings.UTF8);
        //_kbin = File.ReadAllBytes(@"data\test_case.bin");
        _xmlBytes = KbinConverter.ReadXmlBytes(_kbin);
        _linq = KbinConverter.ReadXmlLinq(_kbin);
        _xml = KbinConverter.ReadXml(_kbin);
        _xmlStr = _linq.ToString();
    }

    [Benchmark(Baseline = true)]
    public object? ReadLinq_NKZsmos_32ThreadsX160()
    {
        return MultiThreadUtils.DoMultiThreadWork(_ =>
        {
            return KbinConverter.ReadXmlLinq(_kbin);
        }, 32, 5);
    }

    [Benchmark]
    public object? ReadLinq_FSH_B_32ThreadsX160()
    {
        return MultiThreadUtils.DoMultiThreadWork(_ =>
        {
            return new KbinReader(_kbin).ReadLinq();
        }, 32, 5);
    }

#if NETCOREAPP
    [Benchmark]
    public object? ReadLinq_ItsNovaHere_32ThreadsX160()
    {
        return MultiThreadUtils.DoMultiThreadWork(_ =>
        {
            return new Reader(_kbin).GetDocument();
        }, 32, 5);
    }
#endif
}