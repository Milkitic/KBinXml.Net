using System;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using KbinXml;
using KbinXml.Utils;

namespace ReadBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var bytes = File.ReadAllBytes(@"data\test_case.bin");
            KbinConverter.ReadLinq(bytes);

            //new int[5000].AsParallel().ForAll((i) =>
            //{
            //    reader.ReadLinq();
            //});
            //return;
            var summary = BenchmarkRunner.Run<ReadTask>();
        }
    }

    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    //[SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net48)]
    public class ReadTask
    {
        private byte[] _bytes;

        [GlobalSetup]
        public void Setup()
        {
            _bytes = File.ReadAllBytes(@"data\test_case.bin");
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        [Benchmark]
        public object? New_400KB()
        {
            return KbinConverter.ReadLinq(_bytes);
        }
    }
}
