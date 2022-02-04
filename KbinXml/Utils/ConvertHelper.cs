using System;
using System.Net;

namespace KbinXml.Utils;

public static class ConvertHelper
{
    public static int WriteU8String(ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        builder.Append(byte.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

        return 1;
    }

    public static int WriteS8String(ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        builder.Append((byte)sbyte.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

        return 1;
    }

    public static int WriteU16String(ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        builder,
        ushort.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static int WriteS16String(ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        builder,
        short.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static int WriteU32String(ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        builder,
        uint.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static int WriteS32String(ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        builder,
        int.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static int WriteU64String(ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        builder,
        ulong.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static int WriteS64String(ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        builder,
        long.Parse(str
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static int WriteSingleString(ValueListBuilder<byte> builder, ReadOnlySpan<char> input) => BitConverterHelper.WriteBeBytes(
        builder,
        float.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static int WriteDoubleString(ValueListBuilder<byte> builder, ReadOnlySpan<char> input) => BitConverterHelper.WriteBeBytes(
        builder,
        double.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ));

    public static int WriteIp4String(ValueListBuilder<byte> builder, ReadOnlySpan<char> input)
    {
        var bytes = IPAddress.Parse(input
#if NETSTANDARD2_0
                .ToString()
#endif
        ).GetAddressBytes();

        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i]);
        }

        return bytes.Length;
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