﻿using System;
using System.IO;
using System.Linq;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;
using KbinXml;
using KbinXml.Net;

namespace ReadBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var kbin = KbinConverter.Write(File.ReadAllText(@"data\small.xml"), KnownEncodings.UTF8);
            //var bytes = File.ReadAllBytes(@"data\test_case.bin");
            var xmlBytes = KbinConverter.ReadXmlBytes(kbin);
            var linq = KbinConverter.ReadXmlLinq(kbin);
            var xml = KbinConverter.ReadXml(kbin);
            var xmlStr = linq.ToString();
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
            //_bytes = File.ReadAllBytes(@"data\test_case.bin");
            _bytes = KbinConverter.Write(File.ReadAllText(@"data\small.xml"), KnownEncodings.UTF8);
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
            _target = new int[80];
        }

        [Benchmark]
        public object? NewLinq_400KB()
        {
            return KbinConverter.ReadXmlLinq(_bytes);
        }

        [Benchmark]
        public object? New_400KB()
        {
            return KbinConverter.ReadXml(_bytes);
        }

        [Benchmark]
        public object? NewRaw_400KB()
        {
            return KbinConverter.ReadXmlBytes(_bytes);
        }


        //[Benchmark]
        //public object? NewLinq_400KB_32ThreadsX80()
        //{
        //    return _target
        //        .AsParallel()
        //        .WithDegreeOfParallelism(32)
        //        .Select(_ => KbinConverter.ReadXmlLinq(_bytes))
        //        .ToArray();
        //}


        //[Benchmark]
        //public object? New_400KB_32ThreadsX80()
        //{
        //    return _target
        //        .AsParallel()
        //        .WithDegreeOfParallelism(32)
        //        .Select(_ => KbinConverter.ReadXml(_bytes))
        //        .ToArray();
        //}

        //[Benchmark]
        //public object? NewRaw_400KB_32ThreadsX80()
        //{
        //    return _target
        //        .AsParallel()
        //        .WithDegreeOfParallelism(32)
        //        .Select(_ => KbinConverter.ReadXmlBytes(_bytes))
        //        .ToArray();
        //}
    }
}