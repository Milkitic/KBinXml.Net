using System;
using System.Globalization;
using System.Net;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KbinXml.Net.Internal;

namespace KbinXml.Net.Utils;

internal static class ConvertHelper
{
    private static readonly NumberFormatInfo USNumberFormat = new CultureInfo("en-US", false).NumberFormat;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteU8String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = GetNumberStyle(str, out str);
        builder.Append(ParseHelper.ParseByte(str, numberStyle));
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteS8String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        builder.Append((byte)ParseHelper.ParseSByte(str));
        return 1;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteU16String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt16(str, numberStyle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteS16String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseInt16(str));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteU32String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt32(str, numberStyle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteS32String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseInt32(str));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteU64String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt64(str, numberStyle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteS64String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseInt64(str));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteSingleString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> input)
    {
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseSingle(input, USNumberFormat));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int WriteDoubleString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> input)
    {
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseDouble(input, USNumberFormat));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)] // todo: loop here
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string U8ToString(ReadOnlySpan<byte> bytes)
    {
        return bytes[0].ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string S8ToString(ReadOnlySpan<byte> bytes)
    {
        return ((sbyte)bytes[0]).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string U16ToString(ReadOnlySpan<byte> bytes)
    {
        return BitConverterHelper.ToBeUInt16(bytes).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string S16ToString(ReadOnlySpan<byte> bytes)
    {
        return BitConverterHelper.ToBeInt16(bytes).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string U32ToString(ReadOnlySpan<byte> bytes)
    {
        return BitConverterHelper.ToBeUInt32(bytes).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string S32ToString(ReadOnlySpan<byte> bytes)
    {
        return BitConverterHelper.ToBeInt32(bytes).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string U64ToString(ReadOnlySpan<byte> bytes)
    {
        return BitConverterHelper.ToBeUInt64(bytes).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string S64ToString(ReadOnlySpan<byte> bytes)
    {
        return BitConverterHelper.ToBeInt64(bytes).ToString();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string SingleToString(ReadOnlySpan<byte> bytes)
    {
        return BitConverterHelper.ToBeSingle(bytes).ToString("0.000000");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string DoubleToString(ReadOnlySpan<byte> bytes)
    {
        return BitConverterHelper.ToBeDouble(bytes).ToString("0.000000");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string Ip4ToString(ReadOnlySpan<byte> bytes)
    {
        var privateAddress = MemoryMarshal.Read<uint>(bytes);
        Span<char> dst = stackalloc char[15];
        int charsWritten = IPv4AddressToStringHelper(privateAddress, dst);
        unsafe
        {
            fixed (char* p = dst)
            {
                return new string(p, 0, charsWritten);
            }
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString(ReadOnlySpan<byte> bytes)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        if (bytes.Length > int.MaxValue / 2)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes));
        }

        return HexConverter.ToString(bytes, HexConverter.Casing.Lower);
    }

    [InlineMethod.Inline]
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

    [InlineMethod.Inline]
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

    //[InlineMethod.Inline]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static NumberStyles GetNumberStyle(ReadOnlySpan<char> str, out ReadOnlySpan<char> hex)
    {
        var isSpanHex = str.Length > 2 &&
                        (str[1] == 'x' && str[0] == '0' || str[1] == 'H' && str[0] == '&');

        if (isSpanHex)
        {
            hex = str.Slice(2);
            return NumberStyles.HexNumber;
        }

        hex = str;
        return NumberStyles.Integer;
    }
}