using System;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Utils;

/// <summary>
/// High-performance parsing helpers that operate over <see cref="ReadOnlySpan{Char}"/>
/// where available, minimizing intermediate string allocations.
/// </summary>
public static class ParseHelper
{
    private static readonly string? DoubleMaxString = ((object)double.MaxValue).ToString();

    /// <summary>
    /// Parses a boolean from the provided character span.
    /// </summary>
    /// <param name="input">The input characters representing a boolean.</param>
    /// <returns>The parsed boolean value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool ParseBoolean(ReadOnlySpan<char> input)
    {
#if NET8_0_OR_GREATER
        return bool.Parse(input);
#else
        return bool.Parse(input.ToString());
#endif
    }

    /// <summary>
    /// Parses a byte value using the specified number styles.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="numberStyles">Number styles to use for parsing.</param>
    /// <returns>The parsed byte value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static byte ParseByte(ReadOnlySpan<char> input, NumberStyles numberStyles)
    {
#if NET8_0_OR_GREATER
        return byte.Parse(input, numberStyles);
#else
        return byte.Parse(input.ToString(), numberStyles);
#endif
    }

    /// <summary>
    /// Parses a signed byte from the provided character span.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <returns>The parsed signed byte.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static sbyte ParseSByte(ReadOnlySpan<char> input)
    {
#if NET8_0_OR_GREATER
        return sbyte.Parse(input);
#else
        return sbyte.Parse(input.ToString());
#endif
    }

    /// <summary>
    /// Parses a 16-bit signed integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <returns>The parsed <see cref="short"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ParseInt16(ReadOnlySpan<char> input)
    {
#if NET8_0_OR_GREATER
        return short.Parse(input);
#else
        return short.Parse(input.ToString());
#endif
    }

    /// <summary>
    /// Parses a 16-bit unsigned integer using the specified number styles.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="numberStyles">Number styles to use for parsing.</param>
    /// <returns>The parsed <see cref="ushort"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ParseUInt16(ReadOnlySpan<char> input, NumberStyles numberStyles)
    {
#if NET8_0_OR_GREATER
        return ushort.Parse(input, numberStyles);
#else
        return ushort.Parse(input.ToString(), numberStyles);
#endif
    }

    /// <summary>
    /// Parses a 32-bit signed integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <returns>The parsed <see cref="int"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ParseInt32(ReadOnlySpan<char> input)
    {
#if NET8_0_OR_GREATER
        return int.Parse(input);
#else
        return int.Parse(input.ToString());
#endif
    }

    /// <summary>
    /// Parses a 32-bit unsigned integer using the specified number styles.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="numberStyles">Number styles to use for parsing.</param>
    /// <returns>The parsed <see cref="uint"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ParseUInt32(ReadOnlySpan<char> input, NumberStyles numberStyles)
    {
#if NET8_0_OR_GREATER
        return uint.Parse(input, numberStyles);
#else
        return uint.Parse(input.ToString(), numberStyles);
#endif
    }

    /// <summary>
    /// Parses a 64-bit signed integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <returns>The parsed <see cref="long"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ParseInt64(ReadOnlySpan<char> input)
    {
#if NET8_0_OR_GREATER
        return long.Parse(input);
#else
        return long.Parse(input.ToString());
#endif
    }

    /// <summary>
    /// Parses a 64-bit unsigned integer using the specified number styles.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="numberStyles">Number styles to use for parsing.</param>
    /// <returns>The parsed <see cref="ulong"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ParseUInt64(ReadOnlySpan<char> input, NumberStyles numberStyles)
    {
#if NET8_0_OR_GREATER
        return ulong.Parse(input, numberStyles);
#else
        return ulong.Parse(input.ToString(), numberStyles);
#endif
    }

    /// <summary>
    /// Parses a single-precision floating-point number.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="nfi">Optional number format info to use.</param>
    /// <returns>The parsed <see cref="float"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ParseSingle(ReadOnlySpan<char> input, NumberFormatInfo? nfi = null)
    {
#if NET8_0_OR_GREATER
        return float.Parse(input, provider: nfi);
#else
        return float.Parse(input.ToString(), nfi);
#endif
    }

    /// <summary>
    /// Parses a double-precision floating-point number.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="nfi">Optional number format info to use.</param>
    /// <returns>The parsed <see cref="double"/> value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ParseDouble(ReadOnlySpan<char> input, NumberFormatInfo? nfi = null)
    {
#if NET8_0_OR_GREATER
        return double.Parse(input, provider: nfi);
#else
        var str = input.ToString();
        var d = str == DoubleMaxString ? double.MaxValue : double.Parse(str);
        return d;
#endif
    }

    /// <summary>
    /// Parses an enum value from its string representation.
    /// </summary>
    /// <typeparam name="T">The enum type.</typeparam>
    /// <param name="value">The string representation of the enum value.</param>
    /// <returns>The parsed enum value.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static T ParseEnum<T>(string value) where T : struct
    {
#if NET8_0_OR_GREATER
        return Enum.Parse<T>(value);
#else
        return (T)Enum.Parse(typeof(T), value);
#endif
    }

    /// <summary>
    /// Parses a <see cref="DateTime"/> value from the provided span.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <returns>The parsed date and time.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static DateTime ParseDateTime(ReadOnlySpan<char> input)
    {
#if NET8_0_OR_GREATER
        return DateTime.Parse(input);
#else
        return DateTime.Parse(input.ToString());
#endif
    }

    /// <summary>
    /// Attempts to parse a boolean value.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseBoolean(ReadOnlySpan<char> input, out bool value)
    {
#if NET8_0_OR_GREATER
        return bool.TryParse(input, out value);
#else
        return bool.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a byte value.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseByte(ReadOnlySpan<char> input, out byte value)
    {
#if NET8_0_OR_GREATER
        return byte.TryParse(input, out value);
#else
        return byte.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a signed byte value.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseSByte(ReadOnlySpan<char> input, out sbyte value)
    {
#if NET8_0_OR_GREATER
        return sbyte.TryParse(input, out value);
#else
        return sbyte.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a 16-bit signed integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInt16(ReadOnlySpan<char> input, out short value)
    {
#if NET8_0_OR_GREATER
        return short.TryParse(input, out value);
#else
        return short.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a 16-bit unsigned integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseUInt16(ReadOnlySpan<char> input, out ushort value)
    {
#if NET8_0_OR_GREATER
        return ushort.TryParse(input, out value);
#else
        return ushort.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a 32-bit signed integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInt32(ReadOnlySpan<char> input, out int value)
    {
#if NET8_0_OR_GREATER
        return int.TryParse(input, out value);
#else
        return int.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a 32-bit unsigned integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseUInt32(ReadOnlySpan<char> input, out uint value)
    {
#if NET8_0_OR_GREATER
        return uint.TryParse(input, out value);
#else
        return uint.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a 64-bit signed integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseInt64(ReadOnlySpan<char> input, out long value)
    {
#if NET8_0_OR_GREATER
        return long.TryParse(input, out value);
#else
        return long.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a 64-bit unsigned integer.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseUInt64(ReadOnlySpan<char> input, out ulong value)
    {
#if NET8_0_OR_GREATER
        return ulong.TryParse(input, out value);
#else
        return ulong.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a single-precision floating-point number.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <param name="nfi">Optional number format info to use.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseSingle(ReadOnlySpan<char> input, out float value, NumberFormatInfo? nfi = null)
    {
#if NET8_0_OR_GREATER
        return float.TryParse(input, NumberStyles.Float | NumberStyles.AllowThousands, nfi, out value);
#else
        return float.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a double-precision floating-point number.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <param name="nfi">Optional number format info to use.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseDouble(ReadOnlySpan<char> input, out double value, NumberFormatInfo? nfi = null)
    {
#if NET8_0_OR_GREATER
        return double.TryParse(input, NumberStyles.Float | NumberStyles.AllowThousands, nfi, out value);
#else
        return double.TryParse(input.ToString(), out value);
#endif
    }

    /// <summary>
    /// Attempts to parse a <see cref="DateTime"/> value.
    /// </summary>
    /// <param name="input">The input characters.</param>
    /// <param name="value">When successful, receives the parsed value.</param>
    /// <returns><c>true</c> if parsing succeeded; otherwise <c>false</c>.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool TryParseDateTime(ReadOnlySpan<char> input, out DateTime value)
    {
#if NET8_0_OR_GREATER
        return DateTime.TryParse(input, out value);
#else
        return DateTime.TryParse(input.ToString(), out value);
#endif
    }
}