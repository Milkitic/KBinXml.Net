using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

#if !NETSTANDARD2_0
[SkipLocalsInit]
#endif
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
#if NET8_0_OR_GREATER
        return BinaryPrimitives.ReadSingleBigEndian(value);
#else
        var arr = System.Buffers.ArrayPool<byte>.Shared.Rent(value.Length);
        try
        {
            value.CopyTo(arr);
            var arrSpan = arr.AsSpan(0, value.Length);
            arrSpan.Reverse();
            return BitConverter.ToSingle(arr, 0);
        }
        finally
        {
            System.Buffers.ArrayPool<byte>.Shared.Return(arr);
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToBeDouble(ReadOnlySpan<byte> value)
    {
#if NET8_0_OR_GREATER
        return BinaryPrimitives.ReadDoubleBigEndian(value);
#else
        return BinaryPrimitivesExt.ReadDoubleBigEndian(value);
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytesT<T>(Span<byte> span, T value) where T : unmanaged
    {
        return Type.GetTypeCode(typeof(T)) switch
        {
            TypeCode.UInt16 => WriteBeBytes(span, Unsafe.As<T, ushort>(ref value)),
            TypeCode.Int16 => WriteBeBytes(span, Unsafe.As<T, short>(ref value)),
            TypeCode.UInt32 => WriteBeBytes(span, Unsafe.As<T, uint>(ref value)),
            TypeCode.Int32 => WriteBeBytes(span, Unsafe.As<T, int>(ref value)),
            TypeCode.UInt64 => WriteBeBytes(span, Unsafe.As<T, ulong>(ref value)),
            TypeCode.Int64 => WriteBeBytes(span, Unsafe.As<T, long>(ref value)),
            TypeCode.Single => WriteBeBytes(span, Unsafe.As<T, float>(ref value)),
            TypeCode.Double => WriteBeBytes(span, Unsafe.As<T, double>(ref value)),
            _ => throw new ArgumentOutOfRangeException(nameof(value), typeof(T), "Unsupported type")
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        return sizeof(ushort);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        return sizeof(short);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, uint value)
    {
        BinaryPrimitives.WriteUInt32BigEndian(span, value);
        return sizeof(uint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        return sizeof(int);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, ulong value)
    {
        BinaryPrimitives.WriteUInt64BigEndian(span, value);
        return sizeof(ulong);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        return sizeof(long);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, float value)
    {
#if NET8_0_OR_GREATER
        BinaryPrimitives.WriteSingleBigEndian(span, value);
#else
        BitConverter.GetBytes(value).CopyTo(span);
        span.Reverse();
#endif
        return sizeof(float);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, double value)
    {
#if NET8_0_OR_GREATER
        BinaryPrimitives.WriteDoubleBigEndian(span, value);
#else
        BinaryPrimitivesExt.WriteDoubleBigEndian(span, value);
#endif
        return sizeof(double);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteBeBytes(ref ValueListBuilder<byte> builder, ushort value)
    {
        Span<byte> span = stackalloc byte[sizeof(ushort)];
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        builder.AppendSpan(span);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteBeBytes(ref ValueListBuilder<byte> builder, short value)
    {
        Span<byte> span = stackalloc byte[sizeof(short)];
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        builder.AppendSpan(span);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteBeBytes(ref ValueListBuilder<byte> builder, uint value)
    {
        Span<byte> span = stackalloc byte[sizeof(uint)];
        BinaryPrimitives.WriteUInt32BigEndian(span, value);
        builder.AppendSpan(span);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteBeBytes(ref ValueListBuilder<byte> builder, int value)
    {
        Span<byte> span = stackalloc byte[sizeof(int)];
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        builder.AppendSpan(span);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteBeBytes(ref ValueListBuilder<byte> builder, ulong value)
    {
        Span<byte> span = stackalloc byte[sizeof(ulong)];
        BinaryPrimitives.WriteUInt64BigEndian(span, value);
        builder.AppendSpan(span);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteBeBytes(ref ValueListBuilder<byte> builder, long value)
    {
        Span<byte> span = stackalloc byte[sizeof(long)];
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        builder.AppendSpan(span);
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteBeBytes(ref ValueListBuilder<byte> builder, float value)
    {
#if NET8_0_OR_GREATER
        Span<byte> span = stackalloc byte[sizeof(float)];
        BinaryPrimitives.WriteSingleBigEndian(span, value);
        builder.AppendSpan(span);
#else
        var bytes = BitConverter.GetBytes(value);
        Span<byte> span = bytes;
        span.Reverse();
        builder.AppendSpan(span);
#endif
        return span.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static int WriteBeBytes(ref ValueListBuilder<byte> builder, double value)
    {
        Span<byte> span = stackalloc byte[sizeof(double)];
#if NET8_0_OR_GREATER
        BinaryPrimitives.WriteDoubleBigEndian(span, value);
#else
        BinaryPrimitivesExt.WriteDoubleBigEndian(span, value);
#endif
        builder.AppendSpan(span);
        return span.Length;
    }
}