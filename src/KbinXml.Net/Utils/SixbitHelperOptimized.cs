using System;

namespace KbinXml.Net.Utils;

public static class SixbitHelperOptimized
{
    public static unsafe void EncodeFillOutput(ReadOnlySpan<byte> buffer, ref Span<byte> output)
    {
        fixed (byte* bufferPtr = buffer, outputPtr = output)
        {
            byte* buf = bufferPtr;
            byte* outPtr = outputPtr;
            int length = buffer.Length;
            int globalBitIndex = 0;

            for (int bufIdx = 0; bufIdx < length; bufIdx++, globalBitIndex += 6)
            {
                byte current = buf[bufIdx];
                int outputByte = globalBitIndex >> 3;
                int bitOffset = globalBitIndex & 7; // 等效 globalBitIndex % 8
                int outputBit = 7 - bitOffset;       // 转换为高位优先的位索引

                byte sixBits = (byte)(current & 0x3F); // 提取低6位

                if (outputBit >= 5)
                {
                    // 所有6位在同一个输出字节
                    outPtr[outputByte] |= (byte)(sixBits << (outputBit - 5));
                }
                else
                {
                    // 拆分到两个输出字节
                    int k = outputBit + 1;
                    int remainingBits = 6 - k;

                    // 处理第一个字节的高k位
                    outPtr[outputByte] |= (byte)(sixBits >> (6 - k));

                    // 处理第二个字节的低remainingBits位
                    byte lowPart = (byte)(sixBits & ((1 << remainingBits) - 1));
                    outPtr[outputByte + 1] |= (byte)(lowPart << (7 - (remainingBits - 1)));
                }
            }
        }
    }

    public static unsafe void DecodeFillInput(ReadOnlySpan<byte> buffer, ref Span<byte> input)
    {
        fixed (byte* bufferPtr = buffer, inputPtr = input)
        {
            byte* buf = bufferPtr;
            byte* inPtr = inputPtr;
            int length = input.Length;
            int globalBitIndex = 0;

            for (int bufIdx = 0; bufIdx < length; bufIdx++, globalBitIndex += 6)
            {
                int bufferByte = globalBitIndex >> 3;
                int bitOffset = globalBitIndex & 7;  // 起始位在buffer字节中的位置
                int availableBits = 8 - bitOffset;  // 当前buffer字节剩余可用位数

                byte sixBits;
                if (availableBits >= 6)
                {
                    // 情况1：6位数据在单个buffer字节内
                    sixBits = (byte)((buf[bufferByte] >> (availableBits - 6)) & 0x3F);
                }
                else
                {
                    // 情况2：跨两个buffer字节组合数据
                    int remainingBits = 6 - availableBits;
                    sixBits = (byte)((buf[bufferByte] & ((1 << availableBits) - 1)) << remainingBits);
                    sixBits |= (byte)(buf[bufferByte + 1] >> (8 - remainingBits));
                }

                // 将6位数据存储到输入缓冲区（高位优先）
                inPtr[bufIdx] = sixBits;
            }
        }
    }

    //[InlineMethod.Inline]
    //public static unsafe void EncodeFillOutput_Optimized(ReadOnlySpan<byte> buffer, ref Span<byte> output)
    //{
    //    fixed (byte* bufferPtr = buffer, outputPtr = output)
    //    {
    //        // 遍历输入缓冲区的每个字节（每个字节贡献6位）
    //        for (int bufIdx = 0; bufIdx < buffer.Length; bufIdx++)
    //        {
    //            byte current = bufferPtr[bufIdx];

    //            // 当前字节在输出位流中的起始全局位索引（每字节6位）
    //            var startI = bufIdx * 6;

    //            // 处理当前字节的6个有效位（从高位到低位）
    //            for (int bit = 0; bit < 6; bit++)
    //            {
    //                // 全局位索引：当前字节的起始位 + 当前位偏移
    //                int i = startI + bit;

    //                // 计算输出字节位置：i / 8（等价于右移3位）
    //                int outputByte = i >> 3;
    //                // 计算输出位的目标位置：7 - (i % 8)（高位优先，例如i=0对应字节第7位）
    //                int outputBit = 7 - (i & 7);

    //                // 获取输出字节的指针
    //                byte* ptr = outputPtr + outputByte;

    //                // 编码逻辑：
    //                // 1. 从输入字节提取第(5-bit)位（高位优先，例如bit=0对应第5位）
    //                // 2. 将提取的位移到输出字节的目标位置
    //                // 3. 通过按位或操作合并到输出字节
    //                *ptr |= (byte)(((current >> (5 - bit)) & 1) << outputBit);
    //            }
    //        }
    //    }
    //}

    //[InlineMethod.Inline]
    //public static unsafe void DecodeFillInput_Optimized(ReadOnlySpan<byte> buffer, ref Span<byte> input)
    //{
    //    fixed (byte* bufferPtr = buffer, inputPtr = input)
    //    {
    //        // 遍历输出缓冲区的每个字节（每个字节由6位重构）
    //        for (int bufIdx = 0; bufIdx < input.Length; bufIdx++)
    //        {
    //            byte value = 0;

    //            // 当前字节在输出位流中的起始全局位索引（每字节6位）
    //            var startI = bufIdx * 6;

    //            // 从buffer的多个字节中提取6位，合并到当前输入字节
    //            for (int bit = 0; bit < 6; bit++)
    //            {
    //                // 全局位索引：当前字节的起始位 + 当前位偏移
    //                int i = startI + bit;

    //                // 计算buffer中字节位置：i / 8（等价于右移3位）
    //                int bufferByteIdx = i >> 3;
    //                // 计算buffer中的位位置：7 - (i % 8)（高位优先）
    //                int bufferBitPos = 7 - (i & 7);

    //                // 解码逻辑：
    //                // 1. 从buffer的指定字节提取目标位
    //                // 2. 将提取的位移到输入字节的对应位置（5 - bit）
    //                int bitValue = (bufferPtr[bufferByteIdx] >> bufferBitPos) & 1;
    //                value |= (byte)(bitValue << (5 - bit));
    //            }

    //            // 将重构的6位值写入输入缓冲区
    //            inputPtr[bufIdx] = value;
    //        }
    //    }
    //}
}