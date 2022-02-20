using System;
using System.Linq;
using System.Net;

namespace StableKbin
{
    internal static class Converters
    {
        public delegate Span<byte> StringToByteDelegate(ReadOnlySpan<char> str);
        public delegate string ByteToStringDelegate(Span<byte> bytes);

        public static Span<byte> U8ToBytes(ReadOnlySpan<char> str) => new[] { byte.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ) };
        public static Span<byte> S8ToBytes(ReadOnlySpan<char> str) => new[] { (byte)sbyte.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ) };
        public static Span<byte> U16ToBytes(ReadOnlySpan<char> str) => BitConverterHelper.GetBigEndianBytes(ushort.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));
        public static Span<byte> S16ToBytes(ReadOnlySpan<char> str) => BitConverterHelper.GetBigEndianBytes(short.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));
        public static Span<byte> U32ToBytes(ReadOnlySpan<char> str) => BitConverterHelper.GetBigEndianBytes(uint.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));
        public static Span<byte> S32ToBytes(ReadOnlySpan<char> str) => BitConverterHelper.GetBigEndianBytes(int.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));
        public static Span<byte> U64ToBytes(ReadOnlySpan<char> str) => BitConverterHelper.GetBigEndianBytes(ulong.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));
        public static Span<byte> S64ToBytes(ReadOnlySpan<char> str) => BitConverterHelper.GetBigEndianBytes(long.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));
        public static Span<byte> SingleToBytes(ReadOnlySpan<char> input) => BitConverterHelper.GetBigEndianBytes(float.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ));
        public static Span<byte> DoubleToBytes(ReadOnlySpan<char> input) => BitConverterHelper.GetBigEndianBytes(double.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ));
        public static Span<byte> Ip4ToBytes(ReadOnlySpan<char> input) => IPAddress.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ).GetAddressBytes();
        public static string U8ToString(Span<byte> bytes) => bytes[0].ToString();
        public static string S8ToString(Span<byte> bytes) => ((sbyte)bytes[0]).ToString();
        public static string U16ToString(Span<byte> bytes) => BitConverterHelper.GetBigEndianUInt16(bytes).ToString();
        public static string S16ToString(Span<byte> bytes) => BitConverterHelper.GetBigEndianInt16(bytes).ToString();
        public static string U32ToString(Span<byte> bytes) => BitConverterHelper.GetBigEndianUInt32(bytes).ToString();
        public static string S32ToString(Span<byte> bytes) => BitConverterHelper.GetBigEndianInt32(bytes).ToString();
        public static string U64ToString(Span<byte> bytes) => BitConverterHelper.GetBigEndianUInt64(bytes).ToString();
        public static string S64ToString(Span<byte> bytes) => BitConverterHelper.GetBigEndianInt64(bytes).ToString();
        public static string SingleToString(Span<byte> bytes) => BitConverterHelper.GetBigEndianSingle(bytes).ToString("0.000000");
#if NETSTANDARD2_0
        public static string SingleToStringWithoutCopy(Span<byte> bytes) => BitConverterHelper.GetBigEndianSingleWithoutCopy(bytes).ToString("0.000000");
#endif
        public static string DoubleToString(Span<byte> bytes) => BitConverterHelper.GetBigEndianDouble(bytes).ToString("0.000000");
        public static string Ip4ToString(Span<byte> bytes) => new IPAddress(bytes.ToArray()).ToString();
    }
}