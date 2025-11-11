using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

/// <summary>
/// Utility methods for converting data to and from textual representations
/// commonly used in kbin workflows.
/// </summary>
public static class ConvertHelper
{
    internal static readonly NumberFormatInfo UsNumberFormat = NumberFormatInfo.InvariantInfo;

    /// <summary>
    /// Converts a span of bytes to a hexadecimal string.
    /// </summary>
    /// <param name="bytes">The source bytes.</param>
    /// <param name="upper">Whether to use uppercase letters in the output.</param>
    /// <returns>The hexadecimal string representation.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string ToHexString(ReadOnlySpan<byte> bytes, bool upper = true)
    {
        if (bytes.Length == 0)
        {
            return string.Empty;
        }

        if (bytes.Length > int.MaxValue / 2)
        {
            throw new ArgumentOutOfRangeException(nameof(bytes));
        }

        return HexConverter.ToString(bytes, upper ? HexConverter.Casing.Upper : HexConverter.Casing.Lower);
    }

    /// <summary>
    /// Detects the number style (hex or integer) from the input and returns the
    /// normalized numeric span.
    /// </summary>
    /// <param name="str">The input span which may represent a hex value (e.g., 0xFF, &#38;HFF).</param>
    /// <param name="hex">Receives the span containing only the numeric portion.</param>
    /// <returns>The appropriate <see cref="NumberStyles"/> for parsing.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static NumberStyles GetNumberStyle(ReadOnlySpan<char> str, out ReadOnlySpan<char> hex)
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

    #region Old type converters

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteU8String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    //{
    //    var numberStyle = GetNumberStyle(str, out str);
    //    builder.Append(ParseHelper.ParseByte(str, numberStyle));
    //    return 1;
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteS8String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    //{
    //    builder.Append((byte)ParseHelper.ParseSByte(str));
    //    return 1;
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteU16String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    //{
    //    var numberStyle = GetNumberStyle(str, out str);
    //    return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt16(str, numberStyle));
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteS16String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    //{
    //    return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseInt16(str));
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteU32String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    //{
    //    var numberStyle = GetNumberStyle(str, out str);
    //    return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt32(str, numberStyle));
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteS32String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    //{
    //    return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseInt32(str));
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteU64String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    //{
    //    var numberStyle = GetNumberStyle(str, out str);
    //    return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt64(str, numberStyle));
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteS64String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    //{
    //    return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseInt64(str));
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteSingleString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> input)
    //{
    //    return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseSingle(input, USNumberFormat));
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static int WriteDoubleString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> input)
    //{
    //    return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseDouble(input, USNumberFormat));
    //}

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)] // todo: loop here
    //    public static int WriteIp4String(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> input)
    //    {
    //        var bytes = IPAddress.Parse(input
    //#if !NETCOREAPP3_1_OR_GREATER
    //                .ToString()
    //#endif
    //        ).GetAddressBytes();

    //        for (int i = 0; i < bytes.Length; i++)
    //        {
    //            builder.Append(bytes[i]);
    //        }

    //        return bytes.Length;
    //    }

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string U8ToString(ReadOnlySpan<byte> bytes)
    //{
    //    return bytes[0].ToString();
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string S8ToString(ReadOnlySpan<byte> bytes)
    //{
    //    return ((sbyte)bytes[0]).ToString();
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string U16ToString(ReadOnlySpan<byte> bytes)
    //{
    //    return BitConverterHelper.ToBeUInt16(bytes).ToString();
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string S16ToString(ReadOnlySpan<byte> bytes)
    //{
    //    return BitConverterHelper.ToBeInt16(bytes).ToString();
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string U32ToString(ReadOnlySpan<byte> bytes)
    //{
    //    return BitConverterHelper.ToBeUInt32(bytes).ToString();
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string S32ToString(ReadOnlySpan<byte> bytes)
    //{
    //    return BitConverterHelper.ToBeInt32(bytes).ToString();
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string U64ToString(ReadOnlySpan<byte> bytes)
    //{
    //    return BitConverterHelper.ToBeUInt64(bytes).ToString();
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string S64ToString(ReadOnlySpan<byte> bytes)
    //{
    //    return BitConverterHelper.ToBeInt64(bytes).ToString();
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string SingleToString(ReadOnlySpan<byte> bytes)
    //{
    //    return BitConverterHelper.ToBeSingle(bytes).ToString("0.000000");
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string DoubleToString(ReadOnlySpan<byte> bytes)
    //{
    //    return BitConverterHelper.ToBeDouble(bytes).ToString("0.000000");
    //}

    //[MethodImpl(MethodImplOptions.AggressiveInlining)]
    //public static string Ip4ToString(ReadOnlySpan<byte> bytes)
    //{
    //    var privateAddress = MemoryMarshal.Read<uint>(bytes);
    //    Span<char> dst = stackalloc char[15];
    //    int charsWritten = IPv4AddressToStringHelper(privateAddress, dst);
    //    unsafe
    //    {
    //        fixed (char* p = dst)
    //        {
    //            return new string(p, 0, charsWritten);
    //        }
    //    }
    //}

    #endregion
}
