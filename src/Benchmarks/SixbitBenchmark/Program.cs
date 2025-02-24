using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using KbinXml.Net.Utils;

//BenchmarkRunner.Run<EncodeTask>();
BenchmarkRunner.Run<DecodeTask>();

[MemoryDiagnoser]

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net48)]
public class EncodeTask
{
    private const int Length = 1024 * 1024;

    private byte[] _testData = null!;
    private byte[] _pool = null!;

    [GlobalSetup]
    public void Setup()
    {
        Random rnd = new();
        _testData = new byte[Length];
        rnd.NextBytes(_testData);
        _pool = ArrayPool<byte>.Shared.Rent(Length * 6 / 8);
    }

    [Benchmark]
    public object? EncodeFillOutput_Original()
    {
        Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
        SixbitHelperOriginal.EncodeFillOutput(_testData, ref output);
        return _pool;
    }

    [Benchmark(Baseline = true)]
    public object? EncodeFillOutput_DeepseekAlgorithmOptimized()
    {
        Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
        SixbitHelperOptimized.EncodeFillOutput(_testData, ref output);
        return _pool;
    }

    [Benchmark(Baseline = true)]
    public object? EncodeFillOutput_DeepseekAlgorithmOptimizedDangerous()
    {
        Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
        SixbitHelperReleaseOptimized.EncodeFillOutput(_testData, ref output);
        return _pool;
    }
}

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net48)]
public class DecodeTask
{
    private const int Length = 1024 * 1024;

    private byte[] _testData = null!;
    private byte[] _pool = null!;

    [GlobalSetup]
    public void Setup()
    {
        Random rnd = new();
        var buffer = new byte[Length];
        rnd.NextBytes(buffer);

        Span<byte> output = new byte[Length * 6 / 8];
        SixbitHelperOriginal.EncodeFillOutput(buffer, ref output);

        _testData = output.ToArray();

        _pool = ArrayPool<byte>.Shared.Rent(output.Length * 8 / 6);
    }

    [Benchmark]
    public object? DecodeFillOutput_Original()
    {
        Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
        SixbitHelperOriginal.DecodeFillInput(_testData, ref output);
        return _pool;
    }

    //[Benchmark]
    //public object? DecodeFillOutput_DeepseekOptimized()
    //{
    //    Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
    //    SixbitHelper.DecodeFillInput_Optimized(_testData, ref output);
    //    return _pool;
    //}

    [Benchmark(Baseline = true)]
    public object? DecodeFillOutput_DeepseekAlgorithmOptimized()
    {
        Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
        SixbitHelperOptimized.DecodeFillInput(_testData, ref output);
        return _pool;
    }

    [Benchmark]
    public object? DecodeFillOutput_DeepseekAlgorithmOptimizedDangerous()
    {
        Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
        SixbitHelperReleaseOptimized.DecodeFillInput(_testData, ref output);
        return _pool;
    }
}