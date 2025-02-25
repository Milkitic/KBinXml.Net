using System;
using System.Diagnostics;
using KbinXml.Net.Utils;
using Xunit;

namespace GeneralUnitTests
{
    public class SixbitTest
    {
        [Fact]
        public void VerifyBitEquivalence()
        {
            //var total = 1024 * 1024;

            //Random rnd = new();
            byte[] testData = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            //rnd.NextBytes(testData);

            Span<byte> outputOriginal = new byte[testData.Length * 6 / 8];
            //Span<byte> outputOptimize = new byte[testData.Length * 6 / 8];
            Span<byte> outputOptimizeFinal = new byte[testData.Length * 6 / 8];
            Span<byte> outputOptimizeFinal2 = new byte[testData.Length * 6 / 8];

            // 执行两个版本
            SixbitHelperOriginal.EncodeFillOutput(testData, ref outputOriginal);
            //SixbitHelper.EncodeFillOutput_Optimized(testData, ref outputOptimize);
            SixbitHelperOptimized.Encode(testData, outputOptimizeFinal);
            SixbitHelperReleaseOptimized.Encode(testData, outputOptimizeFinal2);

            // 逐位比较
            for (int i = 0; i < outputOptimizeFinal.Length; i++)
            {
                int originalBit = outputOriginal[i];
                //int optimizeBit = outputOptimize[i];
                int optimizeFinalBit = outputOptimizeFinal[i];
                int optimizeFinal2Bit = outputOptimizeFinal2[i];
                //Debug.Assert(originalBit == optimizeBit,
                //    $"Bit mismatch at position {i}: Original={originalBit}, Optimize={optimizeBit}");
                Debug.Assert(originalBit == optimizeFinalBit,
                    $"Bit mismatch at position {i}: Original={originalBit}, OptimizeFinal={optimizeFinalBit}");
                Debug.Assert(optimizeFinalBit == optimizeFinal2Bit,
                    $"Bit mismatch at position {i}: Original={optimizeFinalBit}, OptimizeFinal2={optimizeFinal2Bit}");
            }
        }

        [Fact]
        public void VerifyBitEquivalence2()
        {
            //var total = 1024 * 1024;

            //Random rnd = new();
            byte[] testData = new byte[] { 0x12, 0x34, 0x56, 0x78 };
            //rnd.NextBytes(testData);
            Span<byte> output = new byte[testData.Length * 6 / 8];
            SixbitHelperOriginal.EncodeFillOutput(testData, ref output);

            Span<byte> inputOriginal = new byte[output.Length * 8 / 6];
            //Span<byte> inputOptimize = new byte[output.Length * 8 / 6];
            Span<byte> inputOptimizeFinal = new byte[output.Length * 8 / 6];
            Span<byte> inputOptimizeFinal2 = new byte[output.Length * 8 / 6];

            // 执行两个版本
            SixbitHelperOriginal.DecodeFillInput(output, ref inputOriginal);
            //SixbitHelper.DecodeFillInput_Optimized(output, ref inputOptimize);
            SixbitHelperOptimized.Decode(output, inputOptimizeFinal);
            SixbitHelperReleaseOptimized.Decode(output, inputOptimizeFinal2);

            // 逐位比较
            for (int i = 0; i < inputOptimizeFinal.Length; i++)
            {
                int originalBit = inputOriginal[i];
                //int optimizeBit = inputOptimize[i];
                int optimizeFinalBit = inputOptimizeFinal[i];
                int optimizeFinal2Bit = inputOptimizeFinal2[i];
                //Debug.Assert(originalBit == optimizeBit,
                //    $"Bit mismatch at position {i}: Original={originalBit}, Optimize={optimizeBit}");
                Debug.Assert(originalBit == optimizeFinalBit,
                    $"Bit mismatch at position {i}: Original={originalBit}, OptimizeFinal={optimizeFinalBit}");
                Debug.Assert(optimizeFinalBit == optimizeFinal2Bit,
                    $"Bit mismatch at position {i}: Original={optimizeFinalBit}, OptimizeFinal2={optimizeFinal2Bit}");
            }
        }
    }
}
