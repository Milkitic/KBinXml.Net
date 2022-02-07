using System;
using System.Net;
using System.Runtime.InteropServices;

namespace KbinXml.Net.Utils;

public static class ConvertHelper
{
    public static int WriteU8String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        builder.Append(byte.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

        return 1;
    }

    public static int WriteS8String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        builder.Append((byte)sbyte.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

        return 1;
    }

    public static int WriteU16String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        ref builder,
        ushort.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

    public static int WriteS16String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        ref builder,
        short.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

    public static int WriteU32String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        ref builder,
        uint.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

    public static int WriteS32String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        ref builder,
        int.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

    public static int WriteU64String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        ref builder,
        ulong.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

    public static int WriteS64String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str) => BitConverterHelper.WriteBeBytes(
        ref builder,
        long.Parse(str
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

    public static int WriteSingleString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> input) => BitConverterHelper.WriteBeBytes(
        ref builder,
        float.Parse(input
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

    public static int WriteDoubleString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> input) => BitConverterHelper.WriteBeBytes(
        ref builder,
        double.Parse(input
#if !NETCOREAPP3_1_OR_GREATER
                .ToString()
#endif
        ));

    public static int WriteIp4String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> input)
    {
        var bytes = IPAddress.Parse(input
#if !NETCOREAPP3_1_OR_GREATER
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
    public static string Ip4ToString(ReadOnlySpan<byte> bytes)
    {
        var privateAddress = MemoryMarshal.Read<uint>(bytes);
        Span<char> dst = stackalloc char[15];
        int charsWritten = IPv4AddressToStringHelper(privateAddress, dst);
        unsafe
        {
            fixed (char* p = dst)
                return new string(p, 0, charsWritten);
        }
    }

    private static int IPv4AddressToStringHelper(uint address, Span<char> dst)
    {
        int offset = 0;
        address = (uint)IPAddress.NetworkToHostOrder(unchecked((int)address));

        FormatIPv4AddressNumber((int)((address >> 24) & 0xFF), dst, ref offset);
        dst[offset++] = '.';
        FormatIPv4AddressNumber((int)((address >> 16) & 0xFF), dst, ref offset);
        dst[offset++] = '.';
        FormatIPv4AddressNumber((int)((address >> 8) & 0xFF), dst, ref offset);
        dst[offset++] = '.';
        FormatIPv4AddressNumber((int)(address & 0xFF), dst, ref offset);

        return offset;
    }

    private static void FormatIPv4AddressNumber(int number, Span<char> dst, ref int offset)
    {
        offset += number > 99 ? 3 : number > 9 ? 2 : 1;

        int i = offset;
        do
        {
            number = Math.DivRem(number, 10, out int rem);
            dst[--i] = (char)('0' + rem);
        } while (number != 0);
    }
}

