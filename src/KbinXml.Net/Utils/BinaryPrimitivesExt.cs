﻿using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KbinXml.Net.Utils;

internal static class BinaryPrimitivesExt
{
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSingleBigEndian(Span<byte> destination, float value)
    {
        if (BitConverter.IsLittleEndian)
        {
            int tmp = BinaryPrimitives.ReverseEndianness(BitConverter.SingleToInt32Bits(value));
#if NET8_0_OR_GREATER
            MemoryMarshal.Write(destination, in tmp);
#else
            MemoryMarshal.Write(destination, ref tmp);
#endif
        }
        else
        {
#if NET8_0_OR_GREATER
            MemoryMarshal.Write(destination, in value);
#else
            MemoryMarshal.Write(destination, ref value);
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadSingleBigEndian(ReadOnlySpan<byte> source)
    {
        return BitConverter.IsLittleEndian
            ? BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<int>(source)))
            : MemoryMarshal.Read<float>(source);
    }
#endif

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDoubleBigEndian(Span<byte> destination, double value)
    {
        if (BitConverter.IsLittleEndian)
        {
            long tmp = BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(value));
#if NET8_0_OR_GREATER
            MemoryMarshal.Write(destination, in tmp);
#else
            MemoryMarshal.Write(destination, ref tmp);
#endif
        }
        else
        {
#if NET8_0_OR_GREATER
            MemoryMarshal.Write(destination, in value);
#else
            MemoryMarshal.Write(destination, ref value);
#endif
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDoubleBigEndian(ReadOnlySpan<byte> source)
    {
        return BitConverter.IsLittleEndian
            ? BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(source)))
            : MemoryMarshal.Read<double>(source);
    }
}