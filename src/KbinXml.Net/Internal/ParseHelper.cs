﻿using System;
using System.Globalization;

namespace KbinXml.Net.Internal;

internal static class ParseHelper
{
    [InlineMethod.Inline]
    public static bool ParseBoolean(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return bool.Parse(input);
#else
        return bool.Parse(input.ToString());
#endif
    }

    [InlineMethod.Inline]
    public static byte ParseByte(ReadOnlySpan<char> input, NumberStyles numberStyles)
    {
#if NETCOREAPP3_1_OR_GREATER
        return byte.Parse(input, numberStyles);
#else
        return byte.Parse(input.ToString(), numberStyles);
#endif
    }

    [InlineMethod.Inline]
    public static sbyte ParseSByte(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return sbyte.Parse(input);
#else
        return sbyte.Parse(input.ToString());
#endif
    }

    [InlineMethod.Inline]
    public static short ParseInt16(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return short.Parse(input);
#else
        return short.Parse(input.ToString());
#endif
    }

    [InlineMethod.Inline]
    public static ushort ParseUInt16(ReadOnlySpan<char> input, NumberStyles numberStyles)
    {
#if NETCOREAPP3_1_OR_GREATER
        return ushort.Parse(input, numberStyles);
#else
        return ushort.Parse(input.ToString(), numberStyles);
#endif
    }

    [InlineMethod.Inline]
    public static int ParseInt32(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return int.Parse(input);
#else
        return int.Parse(input.ToString());
#endif
    }

    [InlineMethod.Inline]
    public static uint ParseUInt32(ReadOnlySpan<char> input, NumberStyles numberStyles)
    {
#if NETCOREAPP3_1_OR_GREATER
        return uint.Parse(input, numberStyles);
#else
        return uint.Parse(input.ToString(), numberStyles);
#endif
    }

    [InlineMethod.Inline]
    public static long ParseInt64(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return long.Parse(input);
#else
        return long.Parse(input.ToString());
#endif
    }

    [InlineMethod.Inline]
    public static ulong ParseUInt64(ReadOnlySpan<char> input, NumberStyles numberStyles)
    {
#if NETCOREAPP3_1_OR_GREATER
        return ulong.Parse(input, numberStyles);
#else
        return ulong.Parse(input.ToString(), numberStyles);
#endif
    }

    [InlineMethod.Inline]
    public static float ParseSingle(ReadOnlySpan<char> input, NumberFormatInfo? nfi = null)
    {
#if NETCOREAPP3_1_OR_GREATER
        return float.Parse(input, provider: nfi);
#else
        return float.Parse(input.ToString(), nfi);
#endif
    }

    [InlineMethod.Inline]
    public static double ParseDouble(ReadOnlySpan<char> input, NumberFormatInfo? nfi = null)
    {
#if NETCOREAPP3_1_OR_GREATER
        return double.Parse(input, provider: nfi);
#else
        return double.Parse(input.ToString(), nfi);
#endif
    }

    [InlineMethod.Inline]
    public static T ParseEnum<T>(string value) where T : struct
    {
#if NETCOREAPP3_1_OR_GREATER
        return Enum.Parse<T>(value);
#else
        return (T)Enum.Parse(typeof(T), value);
#endif
    }

    [InlineMethod.Inline]
    public static DateTime ParseDateTime(ReadOnlySpan<char> input)
    {
#if NETCOREAPP3_1_OR_GREATER
        return DateTime.Parse(input);
#else
        return DateTime.Parse(input.ToString());
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseBoolean(ReadOnlySpan<char> input, out bool value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return bool.TryParse(input, out value);
#else
        return bool.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseByte(ReadOnlySpan<char> input, out byte value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return byte.TryParse(input, out value);
#else
        return byte.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseSByte(ReadOnlySpan<char> input, out sbyte value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return sbyte.TryParse(input, out value);
#else
        return sbyte.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseInt16(ReadOnlySpan<char> input, out short value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return short.TryParse(input, out value);
#else
        return short.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseUInt16(ReadOnlySpan<char> input, out ushort value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return ushort.TryParse(input, out value);
#else
        return ushort.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseInt32(ReadOnlySpan<char> input, out int value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return int.TryParse(input, out value);
#else
        return int.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseUInt32(ReadOnlySpan<char> input, out uint value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return uint.TryParse(input, out value);
#else
        return uint.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseInt64(ReadOnlySpan<char> input, out long value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return long.TryParse(input, out value);
#else
        return long.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseUInt64(ReadOnlySpan<char> input, out ulong value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return ulong.TryParse(input, out value);
#else
        return ulong.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseSingle(ReadOnlySpan<char> input, out float value, NumberFormatInfo? nfi = null)
    {
#if NETCOREAPP3_1_OR_GREATER
        return float.TryParse(input, NumberStyles.Float | NumberStyles.AllowThousands, nfi, out value);
#else
        return float.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseDouble(ReadOnlySpan<char> input, out double value, NumberFormatInfo? nfi = null)
    {
#if NETCOREAPP3_1_OR_GREATER
        return double.TryParse(input, NumberStyles.Float | NumberStyles.AllowThousands, nfi, out value);
#else
        return double.TryParse(input.ToString(), out value);
#endif
    }

    [InlineMethod.Inline]
    public static bool TryParseDateTime(ReadOnlySpan<char> input, out DateTime value)
    {
#if NETCOREAPP3_1_OR_GREATER
        return DateTime.TryParse(input, out value);
#else
        return DateTime.TryParse(input.ToString(), out value);
#endif
    }
}