using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using KbinXml;

namespace ListBenchmark
{
    internal class Program
    {
        static void Main(string[] args)
        {
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

        [GlobalSetup]
        public void Setup()
        {
            var array = Enumerable.Range(0, 256).Select(k => (byte)k).ToArray();
            _bytes = array.Concat(array).ToArray();
        }

        [Benchmark]
        public ReadOnlySpan<byte> ValueListBuilder_Opt()
        {
            var arr = ArrayPool<byte>.Shared.Rent(_bytes.Length);
            var list = new ValueListBuilder<byte>(arr);
            try
            {
                foreach (var b in _bytes)
                {
                    list.Append(b);
                }

                return list.AsSpan();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(arr);
                list.Dispose();
            }
        }

        [Benchmark]
        public ReadOnlySpan<byte> ValueListBuilder()
        {
            var arr = ArrayPool<byte>.Shared.Rent(1);
            var list = new ValueListBuilder<byte>(arr);
            try
            {
                foreach (var b in _bytes)
                {
                    list.Append(b);
                }

                return list.AsSpan();
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(arr);
                list.Dispose();
            }
        }

        [Benchmark]
        public ReadOnlySpan<byte> List()
        {
            var list = new List<byte>();
            foreach (var b in _bytes)
            {
                list.Add(b);
            }

#if NET5_0_OR_GREATER
            return CollectionsMarshal.AsSpan(list);
#else
            return list.ToArray();
#endif
        }

        [Benchmark]
        public ReadOnlySpan<byte> List_Opt()
        {
            var list = new List<byte>(_bytes.Length);
            foreach (var b in _bytes)
            {
                list.Add(b);
            }

#if NET5_0_OR_GREATER
            return CollectionsMarshal.AsSpan(list);
#else
            return list.ToArray();
#endif
        }
    }
}
