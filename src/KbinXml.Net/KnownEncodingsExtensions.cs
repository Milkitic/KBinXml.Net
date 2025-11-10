using System;
using System.Runtime.CompilerServices;
using System.Text;
using KbinXml.Net.Internal;

namespace KbinXml.Net;

/// <summary>
/// Extension methods for converting between <see cref="KnownEncodings"/> and
/// <see cref="System.Text.Encoding"/> instances.
/// </summary>
public static class KnownEncodingsExtensions
{
    /// <summary>
    /// Converts a <see cref="KnownEncodings"/> value to the corresponding
    /// <see cref="Encoding"/> instance.
    /// </summary>
    /// <param name="knownEncodings">The known encoding enumeration value.</param>
    /// <returns>The matching <see cref="Encoding"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the value is not supported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Encoding ToEncoding(this KnownEncodings knownEncodings)
    {
        return knownEncodings switch
        {
            KnownEncodings.ShiftJIS => EncodingDictionary.EncodingShiftJis,
            KnownEncodings.ASCII => Encoding.ASCII,
            KnownEncodings.ISO_8859_1 => EncodingDictionary.EncodingLatin1,
            KnownEncodings.EUC_JP => EncodingDictionary.EncodingEucJp,
            KnownEncodings.UTF8 => Encoding.UTF8,
            _ => throw new ArgumentOutOfRangeException(nameof(knownEncodings), knownEncodings, null)
        };
    }

    /// <summary>
    /// Converts a <see cref="Encoding"/> instance to the corresponding
    /// <see cref="KnownEncodings"/> value.
    /// </summary>
    /// <param name="knownEncodings">The .NET <see cref="Encoding"/> to map.</param>
    /// <returns>The matching <see cref="KnownEncodings"/> value.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the encoding is unsupported.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static KnownEncodings ToKnownEncoding(this Encoding knownEncodings)
    {
        if (knownEncodings.CodePage == EncodingDictionary.EncodingShiftJis.CodePage)
            return KnownEncodings.ShiftJIS;
        if (knownEncodings.CodePage == Encoding.ASCII.CodePage)
            return KnownEncodings.ASCII;
        if (knownEncodings.CodePage == EncodingDictionary.EncodingLatin1.CodePage)
            return KnownEncodings.ISO_8859_1;
        if (knownEncodings.CodePage == EncodingDictionary.EncodingEucJp.CodePage)
            return KnownEncodings.EUC_JP;
        if (knownEncodings.CodePage == Encoding.UTF8.CodePage)
            return KnownEncodings.UTF8;
        throw new ArgumentOutOfRangeException(nameof(knownEncodings), knownEncodings,
            $"Unsupported encoding. CodePage: {knownEncodings.CodePage}, Name: {knownEncodings.EncodingName}");
    }
}