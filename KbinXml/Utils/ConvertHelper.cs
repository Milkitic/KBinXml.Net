using System;
using System.IO;
using System.Net;

namespace KbinXml.Utils;

public static class ConvertHelper
{
    public static void WriteU8String(Stream stream, ReadOnlySpan<char> str) => stream.WriteByte(
        byte.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteS8String(Stream stream, ReadOnlySpan<char> str) => stream.WriteByte(
        (byte)sbyte.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteU16String(Stream stream, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        stream,
        ushort.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteS16String(Stream stream, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        stream,
        short.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteU32String(Stream stream, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        stream,
        uint.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteS32String(Stream stream, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        stream,
        int.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteU64String(Stream stream, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        stream,
        ulong.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteS64String(Stream stream, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        stream,
        long.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteSingleString(Stream stream, ReadOnlySpan<char> input) => BitConverterHelper.WriteBeBytes(
        stream,
        float.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteDoubleString(Stream stream, ReadOnlySpan<char> input) => BitConverterHelper.WriteBeBytes(
        stream,
        double.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static void WriteIp4String(Stream stream, ReadOnlySpan<char> input)
    {
        var bytes = IPAddress.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ).GetAddressBytes();
        stream.Write(bytes, 0, bytes.Length);
    }

    public static string U8ToString(ReadOnlySpan<byte> bytes) => bytes[0].ToString();
    public static string S8ToString(ReadOnlySpan<byte> bytes) => ((sbyte)bytes[0]).ToString();
    public static string U16ToString(ReadOnlySpan<byte> bytes) => BitConverterHelper.ToBeUInt16(bytes).ToString();
    public static string S16ToString(ReadOnlySpan<byte> bytes) => BitConverterHelper.ToBeInt16(bytes).ToString();
    public static string U32ToString(ReadOnlySpan<byte> bytes) => BitConverterHelper.ToBeUInt32(bytes).ToString();
    public static string S32ToString(ReadOnlySpan<byte> bytes) => BitConverterHelper.ToBeInt32(bytes).ToString();
    public static string U64ToString(ReadOnlySpan<byte> bytes) => BitConverterHelper.ToBeUInt64(bytes).ToString();
    public static string S64ToString(ReadOnlySpan<byte> bytes) => BitConverterHelper.ToBeInt64(bytes).ToString();
    public static string SingleToString(ReadOnlySpan<byte> bytes) => BitConverterHelper.ToBeSingle(bytes).ToString("0.000000");
    public static string DoubleToString(ReadOnlySpan<byte> bytes) => BitConverterHelper.ToBeDouble(bytes).ToString("0.000000");
    public static string Ip4ToString(ReadOnlySpan<byte> bytes) => new IPAddress(bytes.ToArray()).ToString();
}