using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using KbinXml;
using KbinXml.Net;

namespace WriteBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            byte[] kbin;
            if (File.Exists(@"d:\data\small.bin"))
                kbin = File.ReadAllBytes(@"d:\data\small.bin");
            else
            {
                kbin = KbinConverter.Write(File.ReadAllText(@"data\small.xml"), Encoding.UTF8);
                File.WriteAllBytes(@"d:\data\small.bin", kbin);
            }
            //var kbin = File.ReadAllBytes(@"data\test_case.bin");
            var xmlBytes = KbinConverter.ReadXmlBytes(kbin);
            var linq = KbinConverter.ReadXmlLinq(kbin);
            var xml = KbinConverter.ReadXml(kbin);
            var xmlStr = linq.ToString();

            var kbin1 = KbinConverter.Write(xmlStr, Encoding.UTF8);
            var linq2 = KbinConverter.ReadXmlLinq(kbin1);
            var xmlBytes2 = KbinConverter.ReadXmlBytes(kbin1);
            var kbin2 = KbinConverter.Write(linq2, Encoding.UTF8);

            //var obj = new object();
            //int i = 0;
            //new int[10000].AsParallel().ForAll(_ =>
            //{
            //    KbinConverter.WriteRaw(str, Encoding.UTF8);
            //    lock (obj)
            //    {
            //        i++;
            //        Console.WriteLine(i);
            //    }
            //});
            //return;

            var summary = BenchmarkRunner.Run<GeneralTask>();
        }
    }


    [Orderer(SummaryOrderPolicy.FastestToSlowest)]
    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net48)]
    public class GeneralTask
    {
        private int[] _target;
        private byte[] _kbin;
        private byte[] _xmlBytes;
        private XDocument _linq;
        private XmlDocument _xml;
        private string _xmlStr;

        [GlobalSetup]
        public void Setup()
        {
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            _kbin = KbinConverter.Write(File.ReadAllText(@"data\small.xml"), Encoding.UTF8);
            //_kbin = File.ReadAllBytes(@"data\test_case.bin");
            _xmlBytes = KbinConverter.ReadXmlBytes(_kbin);
            _linq = KbinConverter.ReadXmlLinq(_kbin);
            _xml = KbinConverter.ReadXml(_kbin);
            _xmlStr = _linq.ToString();

            _target = new int[80];
        }

        [Benchmark]
        public object? WriteRaw_400KB()
        {
            return KbinConverter.Write(_xmlStr, Encoding.UTF8);
        }

        [Benchmark]
        public object? WriteNode_400KB()
        {
            return KbinConverter.Write(_linq, Encoding.UTF8);
        }

        [Benchmark]
        public object? WriteRaw_400KB_32ThreadsX80()
        {
            return _target
                .AsParallel()
                .WithDegreeOfParallelism(32)
                .Select(_ => KbinConverter.Write(_xmlStr, Encoding.UTF8))
                .ToArray();
        }

        [Benchmark]
        public object? WriteLinq_400KB_32ThreadsX80()
        {
            return _target
                .AsParallel()
                .WithDegreeOfParallelism(32)
                .Select(_ => KbinConverter.Write(_linq, Encoding.UTF8))
                .ToArray();
        }
    }
}
