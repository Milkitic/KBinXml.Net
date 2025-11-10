using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace KbinXml.Net.Utils;

/// <summary>
/// Helper methods for reading and writing <see cref="double"/> values in
/// big-endian byte order using low-level primitives.
/// </summary>
public class BinaryPrimitivesExt
{
    /// <summary>
    /// Writes a <see cref="double"/> to <paramref name="destination"/> in big-endian order.
    /// </summary>
    /// <param name="destination">The destination span. Must be at least 8 bytes long.</param>
    /// <param name="value">The value to write.</param>
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

    /// <summary>
    /// Reads a big-endian <see cref="double"/> from <paramref name="source"/>.
    /// </summary>
    /// <param name="source">The source span. Must be at least 8 bytes long.</param>
    /// <returns>The parsed <see cref="double"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDoubleBigEndian(ReadOnlySpan<byte> source)
    {
        return BitConverter.IsLittleEndian
            ? BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(source)))
            : MemoryMarshal.Read<double>(source);
    }
}