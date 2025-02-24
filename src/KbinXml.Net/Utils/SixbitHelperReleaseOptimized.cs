using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

public static class SixbitHelperReleaseOptimized
{
    // 需确保运行环境为小端字节序
    // 要求输入/输出缓冲区预留至少4字节冗余空间（32位操作可能访问后续3字节）
    // 优化后的编码方法，消除条件分支，假设输出缓冲区足够
    /// <summary>
    /// 将6位编码流编码为字节流
    /// </summary>
    /// <param name="buffer">输入缓冲区（每个字节包含6位有效数据）</param>
    /// <param name="output">输出缓冲区（长度应至少为 (input.Length * 6 + 7) / 8）</param>
    public static unsafe void EncodeFillOutput(ReadOnlySpan<byte> buffer, ref Span<byte> output)
    {
        // 参数校验（DEBUG模式下检查缓冲区长度）
        Debug.Assert(output.Length >= (buffer.Length * 6 + 7) >> 3,
            "输出缓冲区长度不足");

        fixed (byte* bufferPtr = buffer, outputPtr = output)
        {
            byte* buf = bufferPtr;
            byte* outPtr = outputPtr;
            int length = buffer.Length;
            int globalBitIndex = 0;

            for (int bufIdx = 0; bufIdx < length; bufIdx++, globalBitIndex += 6)
            {
                /*
                 * 编码策略：
                 * 1. 使用32位操作合并内存访问
                 * 2. 通过预移位消除分支判断
                 * 3. 利用小端序特性合并相邻字节操作
                 */

                // 当前字节的6位有效数据（低6位）
                byte current = (byte)(buf[bufIdx] & 0x3F);

                // 计算全局位偏移
                int outputByte = globalBitIndex >> 3;
                int bitOffset = globalBitIndex & 7;
                int availableBits = 8 - bitOffset;

                // 将6位数据左移到32位的高位区域（26 = 32 - 6）
                uint alignedBits = (uint)current << (26 - bitOffset);

                // 合并写入目标内存（小端序自动处理字节顺序）
                uint* target = (uint*)(outPtr + outputByte);
                *target |= alignedBits >> (26 - availableBits);  // 写入高位部分
                *target |= (alignedBits << availableBits) >> 8;   // 写入低位部分
            }
        }
    }

    /// <summary>
    /// 将字节流解码为6位编码流
    /// </summary>
    /// <param name="buffer">输入字节流</param>
    /// <param name="input">输出缓冲区（长度应至少为 (buffer.Length * 8) / 6）</param>
    public static unsafe void DecodeFillInput(ReadOnlySpan<byte> buffer, ref Span<byte> input)
    {
        // 参数校验（DEBUG模式下检查缓冲区长度）
        Debug.Assert(input.Length >= (buffer.Length << 3) / 6,
            "输入缓冲区长度不足");

        fixed (byte* bufferPtr = buffer, inputPtr = input)
        {
            byte* buf = bufferPtr;
            byte* inPtr = inputPtr;
            int length = input.Length;
            int globalBitIndex = 0;

            for (int bufIdx = 0; bufIdx < length; bufIdx++, globalBitIndex += 6)
            {
                /*
                 * 解码策略：
                 * 1. 批量读取32位数据减少内存访问
                 * 2. 利用位掩码并行提取多个位段
                 * 3. 利用小端序特性合并跨字节操作
                 */

                // 计算全局位偏移
                int bufferByte = globalBitIndex >> 3;
                int bitOffset = globalBitIndex & 7;

                // 读取32位数据块（自动处理跨字节）
                uint chunk = *(uint*)(buf + bufferByte) >> bitOffset;

                // 通过位掩码提取有效位（0x3F000000对应高位6位，0x3F00对应低位6位）
                inPtr[bufIdx] = (byte)(
                    ((chunk & 0x3F000000) >> 24) |  // 提取高位6位
                    ((chunk & 0x00003F00) >> 8)     // 提取低位6位
                );
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