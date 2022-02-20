using System;
using System.Buffers.Binary;
using System.Runtime.InteropServices;

namespace StableKbin
{
    public static class BinaryPrimitivesExt
    {

#if NETSTANDARD2_1 || NET5_0_OR_GREATER
        public static void WriteSingleBigEndian(Span<byte> destination, float value)
        {
            if (BitConverter.IsLittleEndian)
            {
                int tmp = BinaryPrimitives.ReverseEndianness(BitConverter.SingleToInt32Bits(value));
                MemoryMarshal.Write(destination, ref tmp);
            }
            else
            {
                MemoryMarshal.Write(destination, ref value);
            }
        }

        public static float ReadSingleBigEndian(ReadOnlySpan<byte> source)
        {
            return BitConverter.IsLittleEndian
                ? BitConverter.Int32BitsToSingle(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<int>(source)))
                : MemoryMarshal.Read<float>(source);
        }
#endif

        public static void WriteDoubleBigEndian(Span<byte> destination, double value)
        {
            if (BitConverter.IsLittleEndian)
            {
                long tmp = BinaryPrimitives.ReverseEndianness(BitConverter.DoubleToInt64Bits(value));
                MemoryMarshal.Write(destination, ref tmp);
            }
            else
            {
                MemoryMarshal.Write(destination, ref value);
            }
        }

        public static double ReadDoubleBigEndian(ReadOnlySpan<byte> source)
        {
            return BitConverter.IsLittleEndian
                ? BitConverter.Int64BitsToDouble(BinaryPrimitives.ReverseEndianness(MemoryMarshal.Read<long>(source)))
                : MemoryMarshal.Read<double>(source);
        }
    }
}