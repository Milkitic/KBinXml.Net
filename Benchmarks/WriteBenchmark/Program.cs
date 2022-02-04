using System;
using System.IO;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using KbinXml;

namespace WriteBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes(@"data\test_case.bin");
            var str = KbinConverter.ReadLinq(bytes).ToString();
            var backs = KbinConverter.WriteRaw(str, Encoding.UTF8);
            //var sb = KbinConverter.ReadLinq(backs).ToString();
            Console.WriteLine("Hello World!");

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
        private byte[] _bytes;
        private string _str;

        [GlobalSetup]
        public void Setup()
        {
            _bytes = File.ReadAllBytes(@"data\test_case.bin");
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            _str = KbinConverter.ReadLinq(_bytes).ToString();
        }

        [Benchmark]
        public object? New_400KB()
        {
            return KbinConverter.WriteRaw(_str, Encoding.UTF8);
        }
    }
}
