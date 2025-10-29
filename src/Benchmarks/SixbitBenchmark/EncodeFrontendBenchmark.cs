using System;
using System.IO;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Order;
using Microsoft.IO;

namespace SixbitBenchmark;

[MemoryDiagnoser]
[Orderer(SummaryOrderPolicy.FastestToSlowest)]
public class EncodeFrontendBenchmark
{
    private static readonly RecyclableMemoryStreamManager MemoryStreamManager = new();

    [Params(3, 12, 1024)]
    public int Length = 1024 * 1024;

    private string _testData = null!;
    private const string Charset = "0123456789:ABCDEFGHIJKLMNOPQRSTUVWXYZ_abcdefghijklmnopqrstuvwxyz";

    [GlobalSetup]
    public void Setup()
    {
        Random rnd = new(1996);
        char[] chars = new char[Length];
        int charsetLength = Charset.Length;

        for (int i = 0; i < Length; i++)
        {
            chars[i] = Charset[rnd.Next(charsetLength)];
        }

        _testData = new string(chars);
    }
    
    [Benchmark]
    public object? Encode_OldVersion()
    {
        using var ms = new MemoryStream(ushort.MaxValue);
        SixbitBenchmark.SixbitHelper.EncodeAndWrite(ms, _testData);
        return null;
    }


    [Benchmark]
    public object? Encode_NewVersion()
    {
        using var ms = new MemoryStream(ushort.MaxValue);
        KbinXml.Net.Internal.SixbitHelper.EncodeAndWrite(ms, _testData);
        return null;
    }

    [Benchmark(Baseline = true)]
    public object? Encode_OldVersionPool()
    {
        using var ms = MemoryStreamManager.GetStream("test", ushort.MaxValue);
        SixbitBenchmark.SixbitHelper.EncodeAndWrite(ms, _testData);
        return null;
    }

    [Benchmark]
    public object? Encode_NewVersionPool()
    {
        using var ms = MemoryStreamManager.GetStream("test", ushort.MaxValue);
        KbinXml.Net.Internal.SixbitHelper.EncodeAndWrite(ms, _testData);
        return null;
    }
}