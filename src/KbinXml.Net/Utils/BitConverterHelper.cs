using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

/// <summary>
/// Helper methods for reading and writing primitive numeric types in big-endian
/// format and for converting values to their byte representations.
/// </summary>
#if !NETSTANDARD2_0
[SkipLocalsInit]
#endif
public static class BitConverterHelper
{
    /// <summary>
    /// Reads an unsigned 16-bit integer in big-endian order.
    /// </summary>
    /// <param name="readBytes">The source bytes.</param>
    /// <returns>The parsed <see cref="ushort"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ToBeUInt16(ReadOnlySpan<byte> readBytes) =>
        BinaryPrimitives.ReadUInt16BigEndian(readBytes);

    /// <summary>
    /// Reads a signed 16-bit integer in big-endian order.
    /// </summary>
    /// <param name="readBytes">The source bytes.</param>
    /// <returns>The parsed <see cref="short"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ToBeInt16(ReadOnlySpan<byte> readBytes) =>
        BinaryPrimitives.ReadInt16BigEndian(readBytes);

    /// <summary>
    /// Reads an unsigned 32-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The source bytes.</param>
    /// <returns>The parsed <see cref="uint"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ToBeUInt32(ReadOnlySpan<byte> value) =>
        BinaryPrimitives.ReadUInt32BigEndian(value);

    /// <summary>
    /// Reads a signed 32-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The source bytes.</param>
    /// <returns>The parsed <see cref="int"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ToBeInt32(ReadOnlySpan<byte> value) =>
        BinaryPrimitives.ReadInt32BigEndian(value);

    /// <summary>
    /// Reads an unsigned 64-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The source bytes.</param>
    /// <returns>The parsed <see cref="ulong"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ToBeUInt64(ReadOnlySpan<byte> value) =>
        BinaryPrimitives.ReadUInt64BigEndian(value);

    /// <summary>
    /// Reads a signed 64-bit integer in big-endian order.
    /// </summary>
    /// <param name="value">The source bytes.</param>
    /// <returns>The parsed <see cref="long"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ToBeInt64(ReadOnlySpan<byte> value) =>
        BinaryPrimitives.ReadInt64BigEndian(value);

    /// <summary>
    /// Reads a single-precision float in big-endian order.
    /// </summary>
    /// <param name="value">The source bytes.</param>
    /// <returns>The parsed <see cref="float"/>.</returns>
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

    /// <summary>
    /// Reads a double-precision float in big-endian order.
    /// </summary>
    /// <param name="value">The source bytes.</param>
    /// <returns>The parsed <see cref="double"/>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ToBeDouble(ReadOnlySpan<byte> value)
    {
#if NET8_0_OR_GREATER
        return BinaryPrimitives.ReadDoubleBigEndian(value);
#else
        return BinaryPrimitivesExt.ReadDoubleBigEndian(value);
#endif
    }

#if NET8_0_OR_GREATER
    /// <summary>
    /// Writes a primitive integer value of type <typeparamref name="T"/> to the span in big-endian order.
    /// </summary>
    /// <typeparam name="T">An integer type implementing <see cref="System.Numerics.IBinaryInteger{T}"/>.</typeparam>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes<T>(Span<byte> span, T value) where T : System.Numerics.IBinaryInteger<T>
    {
        if (value.TryWriteBigEndian(span, out int bytesWritten))
        {
            return bytesWritten;
        }
        throw new ArgumentException("Span too small");
    }

    /// <summary>
    /// Writes a primitive integer value of type <typeparamref name="T"/> to the builder in big-endian order.
    /// </summary>
    /// <typeparam name="T">An integer type implementing <see cref="System.Numerics.IBinaryInteger{T}"/>.</typeparam>
    /// <param name="builder">The destination builder.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes<T>(ref ValueListBuilder<byte> builder, T value) where T : System.Numerics.IBinaryInteger<T>
    {
        Span<byte> span = stackalloc byte[value.GetByteCount()];
        value.TryWriteBigEndian(span, out int bytesWritten);
        builder.AppendSpan(span);
        return bytesWritten;
    }

    /// <summary>
    /// Reads an integer of type <typeparamref name="T"/> in big-endian order.
    /// </summary>
    /// <typeparam name="T">An integer type implementing <see cref="System.Numerics.IBinaryInteger{T}"/> and <see cref="System.Numerics.IMinMaxValue{T}"/>.</typeparam>
    /// <param name="span">The source bytes.</param>
    /// <returns>The parsed value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ToBe<T>(ReadOnlySpan<byte> span) where T : System.Numerics.IBinaryInteger<T>, System.Numerics.IMinMaxValue<T>
    {
        return T.ReadBigEndian(span, T.IsZero(T.MinValue));
    }
#endif

    /// <summary>
    /// Writes a primitive value of type <typeparamref name="T"/> to the span in big-endian order.
    /// </summary>
    /// <typeparam name="T">An unmanaged primitive type.</typeparam>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
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

    /// <summary>
    /// Writes an unsigned 16-bit integer in big-endian order.
    /// </summary>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(span, value);
        return sizeof(ushort);
    }

    /// <summary>
    /// Writes a signed 16-bit integer in big-endian order.
    /// </summary>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(span, value);
        return sizeof(short);
    }

    /// <summary>
    /// Writes an unsigned 32-bit integer in big-endian order.
    /// </summary>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, uint value)
    {
        BinaryPrimitives.WriteUInt32BigEndian(span, value);
        return sizeof(uint);
    }

    /// <summary>
    /// Writes a signed 32-bit integer in big-endian order.
    /// </summary>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(span, value);
        return sizeof(int);
    }

    /// <summary>
    /// Writes an unsigned 64-bit integer in big-endian order.
    /// </summary>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, ulong value)
    {
        BinaryPrimitives.WriteUInt64BigEndian(span, value);
        return sizeof(ulong);
    }

    /// <summary>
    /// Writes a signed 64-bit integer in big-endian order.
    /// </summary>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteBeBytes(Span<byte> span, long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(span, value);
        return sizeof(long);
    }

    /// <summary>
    /// Writes a single-precision float in big-endian order.
    /// </summary>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
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

    /// <summary>
    /// Writes a double-precision float in big-endian order.
    /// </summary>
    /// <param name="span">The destination span.</param>
    /// <param name="value">The value to write.</param>
    /// <returns>The number of bytes written.</returns>
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