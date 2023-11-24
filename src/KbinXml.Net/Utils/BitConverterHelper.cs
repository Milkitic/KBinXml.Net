using System;
using System.Buffers;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

public static class BitConverterHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ToBeUInt16(ReadOnlySpan<byte> readBytes) =>
        BinaryPrimitives.ReadUInt16BigEndian(readBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ToBeInt16(ReadOnlySpan<byte> readBytes) =>
        BinaryPrimitives.ReadInt16BigEndian(readBytes);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToBeUInt32(ReadOnlySpan<byte> value) =>
        BinaryPrimitives.ReadUInt32BigEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToBeInt32(ReadOnlySpan<byte> value) =>
        BinaryPrimitives.ReadInt32BigEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToBeUInt64(ReadOnlySpan<byte> value) =>
        BinaryPrimitives.ReadUInt64BigEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToBeInt64(ReadOnlySpan<byte> value) =>
        BinaryPrimitives.ReadInt64BigEndian(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ToBeSingle(ReadOnlySpan<byte> value)
    {
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        return BinaryPrimitivesExt.ReadSingleBigEndian(value);
#else
        var arr = ArrayPool<byte>.Shared.Rent(value.Length);
        try
        {
            value.CopyTo(arr);
            var arrSpan = arr.AsSpan(0, value.Length);
            arrSpan.Reverse();
            return BitConverter.ToSingle(arr, 0);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(arr);
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToBeDouble(ReadOnlySpan<byte> value)
    {
        return BinaryPrimitivesExt.ReadDoubleBigEndian(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // todo: `foreach` statements?
    public static int WriteBeBytes(ref ValueListBuilder<byte> builder, ushort value)
    {
        Span<byte> span = stackalloc byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        foreach (var b in span) builder.Append(b);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(ref ValueListBuilder<byte> builder, short value)
    {
        Span<byte> span = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        foreach (var b in span) builder.Append(b);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(ref ValueListBuilder<byte> builder, uint value)
    {
        Span<byte> span = stackalloc byte[sizeof(uint)];
        BinaryPrimitives.WriteUInt32BigEndian(span, value);
        foreach (var b in span) builder.Append(b);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(ref ValueListBuilder<byte> builder, int value)
    {
        Span<byte> span = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        foreach (var b in span) builder.Append(b);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(ref ValueListBuilder<byte> builder, ulong value)
    {
        Span<byte> span = stackalloc byte[sizeof(ulong)];
        BinaryPrimitives.WriteUInt64BigEndian(span, value);
        foreach (var b in span) builder.Append(b);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(ref ValueListBuilder<byte> builder, long value)
    {
        Span<byte> span = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        foreach (var b in span) builder.Append(b);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(ref ValueListBuilder<byte> builder, float value)
    {
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        Span<byte> span = stackalloc byte[sizeof(float)];
        BinaryPrimitivesExt.WriteSingleBigEndian(span, value);
        foreach (var b in span) builder.Append(b);
#else
        var bytes = BitConverter.GetBytes(value);
        Span<byte> span = bytes;
        span.Reverse();
        for (var i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i]);
        }
#endif
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(ref ValueListBuilder<byte> builder, double value)
    {
        Span<byte> span = stackalloc byte[sizeof(double)];
        BinaryPrimitivesExt.WriteDoubleBigEndian(span, value);
        foreach (var b in span)
        {
            builder.Append(b);
        }

        return span.Length;
    }
}