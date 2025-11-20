using System;

namespace KbinXml.Net.Internal.Sixbit;

public static class SixbitHelperSuperOptimized
{
    // 需确保运行环境为小端字节序
    // 要求输入/输出缓冲区预留至少4字节冗余空间（32位操作可能访问后续3字节）
    // 优化后的编码方法，消除条件分支，假设输出缓冲区足够
    /// <summary>
    /// 将6位编码流编码为字节流
    /// </summary>
    /// <param name="buffer">输入缓冲区（每个字节包含6位有效数据）</param>
    /// <param name="output">输出缓冲区（长度应至少为 (input.Length * 6 + 7) / 8）</param>
    public static unsafe void Encode(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        if (buffer.IsEmpty) return;
        int requiredOutputSize = (buffer.Length * 6 + 7) / 8;
        if (output.Length < requiredOutputSize)
            throw new ArgumentException("Output buffer is too small.", nameof(output));

        fixed (byte* bufferPtr = buffer, outputPtr = output)
        {
            byte* buf = bufferPtr;
            byte* outPtr = outputPtr;
            int globalBitIndex = 0;
            int length = buffer.Length;

            for (int bufIdx = 0; bufIdx < length; bufIdx++, globalBitIndex += 6)
            {
                byte sixBits = (byte)(buf[bufIdx] & 0x3F); // 取低6位
                int outputByte = globalBitIndex >> 3;      // 字节索引
                int bitOffset = globalBitIndex & 7;        // 位偏移

                if (bitOffset <= 2) // 6位全在一个字节内
                {
                    outPtr[outputByte] |= (byte)(sixBits << 2 - bitOffset);
                }
                else // 6位跨字节
                {
                    int bitsInFirst = 8 - bitOffset;
                    int bitsInSecond = 6 - bitsInFirst;
                    outPtr[outputByte] |= (byte)(sixBits >> bitsInSecond);
                    if (outputByte + 1 < output.Length)
                    {
                        outPtr[outputByte + 1] |= (byte)((sixBits & (1 << bitsInSecond) - 1) << 8 - bitsInSecond);
                    }
                }
            }
        }
    }

    /// <summary>
    /// 将字节流解码为6位编码流
    /// </summary>
    /// <param name="buffer">输入字节流</param>
    /// <param name="input">输出缓冲区（长度应至少为 (buffer.Length * 8) / 6）</param>
    public static unsafe void Decode(ReadOnlySpan<byte> buffer, Span<byte> input)
    {
        if (buffer.IsEmpty) return;
        int maxOutputLength = buffer.Length * 8 / 6;
        if (input.Length > maxOutputLength)
            throw new ArgumentException("Input buffer capacity exceeds maximum decodable length.", nameof(input));

        fixed (byte* bufferPtr = buffer, inputPtr = input)
        {
            byte* buf = bufferPtr;
            byte* inPtr = inputPtr;
            int globalBitIndex = 0;
            int length = input.Length;

            for (int bufIdx = 0; bufIdx < length; bufIdx++, globalBitIndex += 6)
            {
                int bufferByte = globalBitIndex >> 3;
                int bitOffset = globalBitIndex & 7;
                int availableBits = 8 - bitOffset;

                byte sixBits;
                if (availableBits >= 6) // 6位全在一个字节内
                {
                    sixBits = (byte)(buf[bufferByte] >> availableBits - 6 & 0x3F);
                }
                else // 6位跨字节
                {
                    int bitsFromFirst = availableBits;
                    int bitsFromSecond = 6 - bitsFromFirst;
                    sixBits = (byte)((buf[bufferByte] & (1 << bitsFromFirst) - 1) << bitsFromSecond);
                    if (bufferByte + 1 < buffer.Length)
                    {
                        sixBits |= (byte)(buf[bufferByte + 1] >> 8 - bitsFromSecond);
                    }
                }

                inPtr[bufIdx] = sixBits;
            }
        }
    }

    //// 优化后的编码方法，消除条件分支，假设输出缓冲区足够
    //[InlineMethod.Inline]
    //public static unsafe void EncodeFillOutput(ReadOnlySpan<byte> buffer, ref Span<byte> output)
    //{
    //    fixed (byte* bufferPtr = buffer, outputPtr = output)
    //    {
    //        byte* buf = bufferPtr;
    //        byte* outPtr = outputPtr;
    //        int length = buffer.Length;
    //        int globalBitIndex = 0;

    //        for (int bufIdx = 0; bufIdx < length; bufIdx++, globalBitIndex += 6)
    //        {
    //            byte current = buf[bufIdx];
    //            int outputByte = globalBitIndex >> 3;
    //            int bitOffset = globalBitIndex & 7;
    //            int availableBits = 8 - bitOffset;

    //            // 提取低6位并直接计算掩码
    //            uint sixBits = (uint)(current & 0x3F) << (26 - bitOffset); // 左移使高位对齐到可用空间

    //            // 使用32位写入来合并相邻操作
    //            uint* ptr = (uint*)(outPtr + outputByte);
    //            *ptr |= sixBits >> (26 - availableBits); // 高位部分
    //            *ptr |= (sixBits << availableBits) >> 8; // 低位部分自动跨字节
    //        }
    //    }
    //}

    //// 优化后的解码方法，利用预取和合并读取
    //[InlineMethod.Inline]
    //public static unsafe void DecodeFillInput(ReadOnlySpan<byte> buffer, ref Span<byte> input)
    //{
    //    fixed (byte* bufferPtr = buffer, inputPtr = input)
    //    {
    //        byte* buf = bufferPtr;
    //        byte* inPtr = inputPtr;
    //        int length = input.Length;
    //        int globalBitIndex = 0;

    //        for (int bufIdx = 0; bufIdx < length; bufIdx++, globalBitIndex += 6)
    //        {
    //            int bufferByte = globalBitIndex >> 3;
    //            int bitOffset = globalBitIndex & 7;

    //            // 一次性读取32位处理跨字节情况
    //            uint chunk = *(uint*)(buf + bufferByte) >> bitOffset;
    //            inPtr[bufIdx] = (byte)((chunk & 0x3F000000) >> 24 | (chunk & 0x3F00) >> 8);
    //        }
    //    }
    //}
}