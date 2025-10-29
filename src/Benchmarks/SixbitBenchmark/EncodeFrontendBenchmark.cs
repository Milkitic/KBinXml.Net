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
    public int Length;

    private string _testData = null!;

    private MemoryStream _msOld = null!;
    private MemoryStream _msNew = null!;
    private RecyclableMemoryStream _pmsOld = null!;
    private RecyclableMemoryStream _pmsNew = null!;

    private int _estimatedCapacity;
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
        _estimatedCapacity = (int)(Length * 0.75) + 16;
        _msOld = new MemoryStream(_estimatedCapacity);
        _msNew = new MemoryStream(_estimatedCapacity);
        _pmsOld = MemoryStreamManager.GetStream("test", _estimatedCapacity);
        _pmsNew = MemoryStreamManager.GetStream("test", _estimatedCapacity);
    }

    [Benchmark]
    public object? Encode_OldVersion()
    {
        _msOld.SetLength(0);
        SixbitBenchmark.SixbitHelper.EncodeAndWrite(_msOld, _testData);
        return null;
    }

    [Benchmark]
    public object? Encode_NewVersion()
    {
        _msNew.SetLength(0);
        KbinXml.Net.Internal.SixbitHelper.EncodeAndWrite(_msNew, _testData);
        return null;
    }

    [Benchmark(Baseline = true)]
    public object? Encode_OldVersionPool()
    {
        _pmsOld.SetLength(0);
        SixbitBenchmark.SixbitHelper.EncodeAndWrite(_pmsOld, _testData);
        return null;
    }

    [Benchmark]
    public object? Encode_NewVersionPool()
    {
        _pmsNew.SetLength(0);
        KbinXml.Net.Internal.SixbitHelper.EncodeAndWrite(_pmsNew, _testData);
        return null;
    }

    [GlobalCleanup]
    public void Cleanup()
    {
        _msOld.Dispose();
        _msNew.Dispose();
        _pmsOld.Dispose();
        _pmsNew.Dispose();
    }
}