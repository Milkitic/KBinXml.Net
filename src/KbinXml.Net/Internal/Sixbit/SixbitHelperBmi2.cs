using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
#if NET8_0_OR_GREATER
using System.Runtime.Intrinsics.X86;
#endif

namespace KbinXml.Net.Internal.Sixbit;

/// <summary>
/// 使用 BMI2 (PEXT/PDEP) 指令集优化的 Sixbit 编码/解码帮助类。
/// 适用于支持 AVX2/BMI2 的现代 CPU (Haswell 及更高版本)。
/// </summary>
internal static class SixbitHelperBmi2
{
    // 掩码：00111111 00111111 00111111 00111111
    // 用于一次性提取/放置4个字节的低6位
    private const uint SixBitMask = 0x3F3F3F3F;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Encode(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
#if NET8_0_OR_GREATER
        // 1. 优先尝试 BMI2 (极速路径)
        if (Bmi2.IsSupported)
        {
            EncodeBmi2(buffer, output);
        }
        else
#endif
        {
            // 2. 回退到无分支 Scalar 实现
            EncodeScalarUnrolled(buffer, output);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void Decode(ReadOnlySpan<byte> buffer, Span<byte> input)
    {
#if NET8_0_OR_GREATER
        // 1. 优先尝试 BMI2 (极速路径)
        if (Bmi2.IsSupported)
        {
            DecodeBmi2(buffer, input);
        }
        else
#endif
        {
            // 2. 回退到无分支 Scalar 实现
            DecodeScalarUnrolled(buffer, input);
        }
    }

#if NET8_0_OR_GREATER
    private static void EncodeBmi2(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        ref byte srcRef = ref MemoryMarshal.GetReference(buffer);
        ref byte dstRef = ref MemoryMarshal.GetReference(output);

        int srcIdx = 0;
        int dstIdx = 0;
        int srcLen = buffer.Length;
        int dstLen = output.Length;

        // 主循环：每次处理 4 个输入字节 -> 生成 3 个输出字节
        // 确保读取 4 字节不会越界，写入 4 字节（实际有效3字节）不会越界
        // 注意：Unsafe.WriteUnaligned 会写4字节，所以输出缓冲区必须留有余量或最后处理
        while (srcIdx <= srcLen - 4 && dstIdx <= dstLen - 4)
        {
            // 1. 读取 4 个字节 (Little Endian: D C B A)
            uint input = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref srcRef, srcIdx));

            // 2. 转换为 Big Endian (A B C D) 以便按顺序提取位
            if (BitConverter.IsLittleEndian)
            {
                input = BinaryPrimitives.ReverseEndianness(input);
            }

            // 3. 并行位提取 (PEXT)
            // 输入: 00aaaaaa 00bbbbbb 00cccccc 00dddddd
            // 结果: 00000000 aaaaaabb bbbbcccc ccdddddd (24位紧凑数据)
            uint packed = Bmi2.ParallelBitExtract(input, SixBitMask);

            // 4. 写入结果
            // PEXT 结果在寄存器低位。为了按 Big Endian 顺序写入内存 (High byte first)，
            // 我们再次反转字节序。
            // 寄存器: 00 A B C -> 反转 -> C B A 00
            if (BitConverter.IsLittleEndian)
            {
                packed = BinaryPrimitives.ReverseEndianness(packed);
            }

            // 5. 写入 3 个有效字节
            // packed 现在是 [Byte2, Byte1, Byte0, 00] (内存顺序)
            // 我们需要丢弃最低字节（原本的高位空字节），所以右移 8 位
            // 结果: [00, Byte2, Byte1, Byte0] -> 写入后内存为 Byte0, Byte1, Byte2 (正确顺序)
            // 实际上：ReverseEndianness 后是 [C B A 00] (in register value term? No.)
            // 让我们理清：
            // Val = 0x00ABCDEF. Reverse -> 0xEFCDAB00.
            // Write Memory: EF CD AB 00.
            // 我们想要写入: AB CD EF.
            // 所以我们需要写入的值在内存中是 AB CD EF xx.
            // 也就是 0x...EFCDAB. 
            // 所以：Reverse(Val) >> 8 => 0x00EFCDAB. 
            // Write => AB CD EF 00. 
            Unsafe.WriteUnaligned(ref Unsafe.Add(ref dstRef, dstIdx), packed >> 8);

            srcIdx += 4;
            dstIdx += 3;
        }

        // 处理剩余部分 (Scalar Fallback)
        // 因为 BMI2 路径需要安全的 4 字节读写，不足的部分用慢速循环补齐
        if (srcIdx < srcLen)
        {
            EncodeScalarUnrolled(buffer.Slice(srcIdx), output.Slice(dstIdx));
        }
    }

    private static void DecodeBmi2(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        ref byte srcRef = ref MemoryMarshal.GetReference(buffer);
        ref byte dstRef = ref MemoryMarshal.GetReference(output);

        int srcIdx = 0;
        int dstIdx = 0;
        int srcLen = buffer.Length;
        int dstLen = output.Length;

        // 主循环：每次读取 4 字节（实际使用 3 字节）-> 生成 4 字节
        // 要求源必须至少有 4 字节可读（为了 Unsafe.ReadUnaligned<uint> 安全）
        while (srcIdx <= srcLen - 4 && dstIdx <= dstLen - 4)
        {
            // 1. 读取 4 个字节 (我们只需要前 3 个)
            // Memory: [B0 B1 B2 X]
            uint packed = Unsafe.ReadUnaligned<uint>(ref Unsafe.Add(ref srcRef, srcIdx));

            // 2. 调整数据位置
            // LE读取后寄存器: X B2 B1 B0
            // Reverse -> B0 B1 B2 X
            // Shift >> 8 -> 00 B0 B1 B2 (值: 0x00ABCDEF)
            if (BitConverter.IsLittleEndian)
            {
                packed = BinaryPrimitives.ReverseEndianness(packed) >> 8;
            }
            else
            {
                packed >>= 8; // BE 机器直接右移对齐
            }

            // 3. 并行位分散 (PDEP)
            // 输入: 00000000 aaaaaabb bbbbcccc ccdddddd
            // 掩码: 00111111 00111111 00111111 00111111
            // 结果: 00aaaaaa 00bbbbbb 00cccccc 00dddddd (00 A B C D)
            uint unpacked = Bmi2.ParallelBitDeposit(packed, SixBitMask);

            // 4. 写入结果
            // 我们希望内存顺序: A B C D
            // 当前寄存器值: 0x00ABCDEF (假设 A是高位)
            // 实际上 PDEP 结果中 A 是高位。
            // Reverse -> 0xEFCDAB00. (D C B A).
            // Write LE -> A B C D. Correct.
            if (BitConverter.IsLittleEndian)
            {
                unpacked = BinaryPrimitives.ReverseEndianness(unpacked);
            }

            Unsafe.WriteUnaligned(ref Unsafe.Add(ref dstRef, dstIdx), unpacked);

            srcIdx += 3;
            dstIdx += 4;
        }

        // 处理剩余部分
        if (dstIdx < dstLen)
        {
            DecodeScalarUnrolled(buffer.Slice(srcIdx), output.Slice(dstIdx));
        }
    }
    
#endif
    public static unsafe void EncodeScalarUnrolled(ReadOnlySpan<byte> buffer, Span<byte> output)
    {
        fixed (byte* srcPtr = buffer, dstPtr = output)
        {
            byte* src = srcPtr;
            byte* dst = dstPtr;
            int len = buffer.Length;
            int i = 0;
            int j = 0;

            // 循环展开：每次处理 4 个输入字节，产生 3 个输出字节
            // 这样可以硬编码位移，消除所有 if/else 判断
            while (i <= len - 4)
            {
                byte b0 = src[i];
                byte b1 = src[i + 1];
                byte b2 = src[i + 2];
                byte b3 = src[i + 3];

                // 逻辑：
                // Byte0: [AAAAAA] [BB]
                // Byte1: [BBBB] [CCCC]
                // Byte2: [CC] [DDDDDD]

                dst[j] = (byte)((b0 << 2) | ((b1 & 0x30) >> 4));
                dst[j + 1] = (byte)(((b1 & 0x0F) << 4) | ((b2 & 0x3C) >> 2));
                dst[j + 2] = (byte)(((b2 & 0x03) << 6) | (b3 & 0x3F));

                i += 4;
                j += 3;
            }

            // 处理尾部剩余 (0-3 个字节)
            // 这里使用 switch/case 只会发生一次，不会影响整体性能
            if (i < len)
            {
                byte b0 = src[i];
                dst[j] = (byte)(b0 << 2);
                if (i + 1 < len)
                {
                    byte b1 = src[i + 1];
                    dst[j] |= (byte)((b1 & 0x30) >> 4);
                    dst[j + 1] = (byte)((b1 & 0x0F) << 4);

                    if (i + 2 < len)
                    {
                        byte b2 = src[i + 2];
                        dst[j + 1] |= (byte)((b2 & 0x3C) >> 2);
                        dst[j + 2] = (byte)((b2 & 0x03) << 6);
                        // 不会有 i+3，否则会进入 while 循环
                    }
                }
            }
        }
    }

    public static unsafe void DecodeScalarUnrolled(ReadOnlySpan<byte> buffer, Span<byte> input)
    {
        fixed (byte* srcPtr = buffer, dstPtr = input)
        {
            byte* src = srcPtr;
            byte* dst = dstPtr;
            int dstLen = input.Length; // 目标（解码后）的长度
            int i = 0; // src index
            int j = 0; // dst index

            // 循环展开：每次读取 3 个源字节，写入 4 个目标字节
            // 只有当目标至少还有 4 个位置时才进行展开处理
            while (j <= dstLen - 4)
            {
                byte b0 = src[i];
                byte b1 = src[i + 1];
                byte b2 = src[i + 2];

                // 还原逻辑：
                // Out0: (B0 >> 2) & 0x3F
                // Out1: ((B0 & 0x03) << 4) | ((B1 >> 4) & 0x0F)
                // Out2: ((B1 & 0x0F) << 2) | ((B2 >> 6) & 0x03)
                // Out3: B2 & 0x3F

                dst[j] = (byte)((b0 >> 2) & 0x3F);
                dst[j + 1] = (byte)(((b0 & 0x03) << 4) | ((b1 >> 4) & 0x0F));
                dst[j + 2] = (byte)(((b1 & 0x0F) << 2) | ((b2 >> 6) & 0x03));
                dst[j + 3] = (byte)(b2 & 0x3F);

                i += 3;
                j += 4;
            }

            // 处理尾部
            if (j < dstLen)
            {
                // 至少有1个字节需要写入
                byte b0 = src[i];
                dst[j] = (byte)((b0 >> 2) & 0x3F);

                if (j + 1 < dstLen)
                {
                    // 需要第2个输出，可能需要读第2个输入
                    byte b1 = (i + 1 < buffer.Length) ? src[i + 1] : (byte)0;
                    dst[j + 1] = (byte)(((b0 & 0x03) << 4) | ((b1 >> 4) & 0x0F));

                    if (j + 2 < dstLen)
                    {
                        // 需要第3个输出，可能需要读第3个输入
                        byte b2 = (i + 2 < buffer.Length) ? src[i + 2] : (byte)0;
                        dst[j + 2] = (byte)(((b1 & 0x0F) << 2) | ((b2 >> 6) & 0x03));
                    }
                }
            }
        }
    }
}