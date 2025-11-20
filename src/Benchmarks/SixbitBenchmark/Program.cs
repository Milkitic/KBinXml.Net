using System;
using System.Buffers;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Order;
using BenchmarkDotNet.Running;
using KbinXml.Net.Internal.Sixbit;

BenchmarkRunner.Run<EncodeTask>();
//BenchmarkRunner.Run<EncodeFrontendBenchmark>();

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net48)]
[SimpleJob(RuntimeMoniker.Net80)]
[SimpleJob(RuntimeMoniker.Net90)]
[SimpleJob(RuntimeMoniker.Net10_0)]
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

    //[Benchmark]
    //public object? Encode_Original()
    //{
    //    Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
    //    SixbitHelperOriginal.Encode(_testData, output);
    //    return _pool;
    //}

    //[Benchmark(Baseline = true)]
    //public object? Encode_AlgorithmOptimized()
    //{
    //    Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
    //    SixbitHelperOptimized.Encode(_testData, output);
    //    return _pool;
    //}

    [Benchmark]
    public object? Encode_AlgorithmSuperOptimized()
    {
        Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
        SixbitHelperSuperOptimized.Encode(_testData, output);
        return _pool;
    }

    [Benchmark]
    public object? Encode_AlgorithmCoreClrOptimized()
    {
        Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
        SixbitHelperCoreClrOptimized.Encode(_testData, output);
        return _pool;
    }

    [Benchmark(Baseline = true)]
    public object? Encode_AlgorithmBmi2()
    {
        Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
        SixbitHelperBmi2.Encode(_testData, output);
        return _pool;
    }

    [Benchmark]
    public object? Encode_AlgorithmScalarUnrolled()
    {
        Span<byte> output = _pool.AsSpan(0, Length * 6 / 8);
        SixbitHelperBmi2.EncodeScalarUnrolled(_testData, output);
        return _pool;
    }
}

[Orderer(SummaryOrderPolicy.FastestToSlowest)]
[SimpleJob(RuntimeMoniker.Net90)]
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
        SixbitHelperOriginal.Encode(buffer, output);

        _testData = output.ToArray();

        _pool = ArrayPool<byte>.Shared.Rent(output.Length * 8 / 6);
    }

    //[Benchmark]
    //public object? Decode_Original()
    //{
    //    Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
    //    SixbitHelperOriginal.DecodeFillInput(_testData, ref output);
    //    return _pool;
    //}

    //[Benchmark]
    //public object? Decode_Optimized()
    //{
    //    Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
    //    SixbitHelper.DecodeFillInput_Optimized(_testData, ref output);
    //    return _pool;
    //}

    [Benchmark(Baseline = true)]
    public object? Decode_AlgorithmOptimized()
    {
        Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
        SixbitHelperOptimized.Decode(_testData, output);
        return _pool;
    }

    [Benchmark]
    public object? Decode_AlgorithmSuperOptimized()
    {
        Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
        SixbitHelperSuperOptimized.Decode(_testData, output);
        return _pool;
    }

    [Benchmark]
    public object? Decode_AlgorithmCoreClrOptimized()
    {
        Span<byte> output = _pool.AsSpan(0, _testData.Length * 6 / 8);
        SixbitHelperCoreClrOptimized.Decode(_testData, output);
        return _pool;
    }
}