using KbinXml.Net.Internal.Sixbit;

namespace ComponentsUnitTests;

public class SixbitTests
{
    [Fact]
    public void VerifyBitEquivalence()
    {
        //byte[] testData = new byte[] { 0x12, 0x34, 0x56, 0x78 };

        var total = 1024 * 1024;
        Random rnd = new();
        byte[] testData = new byte[total];
        rnd.NextBytes(testData);

        Span<byte> outputOriginal = new byte[testData.Length * 6 / 8];
        Span<byte> outputOptimized = new byte[testData.Length * 6 / 8];
        Span<byte> outputSuperOptimized = new byte[testData.Length * 6 / 8];
        Span<byte> outputCoreClrOptimized = new byte[testData.Length * 6 / 8];

        // Execute both versions
        SixbitHelperOriginal.Encode(testData, outputOriginal);
        SixbitHelperOptimized.Encode(testData, outputOptimized);
        SixbitHelperSuperOptimized.Encode(testData, outputSuperOptimized);
        SixbitHelperCoreClrOptimized.Encode(testData, outputCoreClrOptimized);

        Assert.Equal(outputOriginal.ToArray(), outputOptimized.ToArray());
        Assert.Equal(outputOriginal.ToArray(), outputSuperOptimized.ToArray());
        Assert.Equal(outputOriginal.ToArray(), outputCoreClrOptimized.ToArray());
    }

    [Fact]
    public void VerifyBitEquivalence2()
    {
        //byte[] testData = new byte[] { 0x12, 0x34, 0x56, 0x78 };

        var total = 1024 * 1024;
        Random rnd = new();
        byte[] testData = new byte[total];
        rnd.NextBytes(testData);

        Span<byte> output = new byte[testData.Length * 6 / 8];
        SixbitHelperOriginal.Encode(testData, output);

        Span<byte> inputOriginal = new byte[output.Length * 8 / 6];
        Span<byte> inputOptimized = new byte[output.Length * 8 / 6];
        Span<byte> inputSuperOptimized = new byte[output.Length * 8 / 6];
        Span<byte> inputCoreClrOptimized = new byte[output.Length * 8 / 6];


        // Execute both versions
        SixbitHelperOriginal.Decode(output, inputOriginal);
        SixbitHelperOptimized.Decode(output, inputOptimized);
        SixbitHelperSuperOptimized.Decode(output, inputSuperOptimized);
        SixbitHelperCoreClrOptimized.Decode(output, inputCoreClrOptimized);

        Assert.Equal(inputOriginal.ToArray(), inputOptimized.ToArray());
        Assert.Equal(inputOriginal.ToArray(), inputSuperOptimized.ToArray());
        Assert.Equal(inputSuperOptimized.ToArray(), inputCoreClrOptimized.ToArray());
    }
}