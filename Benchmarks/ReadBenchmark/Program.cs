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
            KbinConverter.ReadXmlByte(bytes);
            KbinConverter.ReadLinq(bytes);

            //new int[5000].AsParallel().ForAll((i) =>
            //{
            //    KbinConverter.ReadXmlByte(bytes);
            //});
            //return;

            var summary = BenchmarkRunner.Run<ReadTask>();
        }
    }

    [MemoryDiagnoser]
    [SimpleJob(RuntimeMoniker.Net60)]
    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [SimpleJob(RuntimeMoniker.Net48)]
    public class ReadTask
    {
        private byte[] _bytes;
        private int[] _target;

        [GlobalSetup]
        public void Setup()
        {
            _bytes = File.ReadAllBytes(@"data\test_case.bin");
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            _target = new int[80];
        }

        [Benchmark]
        public object? NewLinq_400KB()
        {
            return KbinConverter.ReadLinq(_bytes);
        }

        [Benchmark]
        public object? NewRaw_400KB()
        {
            return KbinConverter.ReadXmlByte(_bytes);
        }


        [Benchmark]
        public object? NewLinq_400KB_32ThreadsX80()
        {
            return _target
                .AsParallel()
                .WithDegreeOfParallelism(32)
                .Select(_ => KbinConverter.ReadLinq(_bytes))
                .ToArray();
        }

        [Benchmark]
        public object? NewRaw_400KB_32ThreadsX80()
        {
            return _target
                .AsParallel()
                .WithDegreeOfParallelism(32)
                .Select(_ => KbinConverter.ReadXmlByte(_bytes))
                .ToArray();
        }
    }
}
