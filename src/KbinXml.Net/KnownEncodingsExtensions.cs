using System;
using System.Runtime.CompilerServices;
using System.Text;
using KbinXml.Net.Internal;

namespace KbinXml.Net;

public static class KnownEncodingsExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Encoding ToEncoding(this KnownEncodings knownEncodings)
    {
        return knownEncodings switch
        {
            KnownEncodings.ShiftJIS => EncodingDictionary.EncodingMap[0x80],
            KnownEncodings.ASCII => EncodingDictionary.EncodingMap[0x20],
            KnownEncodings.ISO_8859_1 => EncodingDictionary.EncodingMap[0x40],
            KnownEncodings.EUC_JP => EncodingDictionary.EncodingMap[0x60],
            KnownEncodings.UTF8 => EncodingDictionary.EncodingMap[0xA0],
            _ => throw new ArgumentOutOfRangeException(nameof(knownEncodings), knownEncodings, null)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static KnownEncodings ToKnownEncoding(this Encoding knownEncodings)
    {
        if (knownEncodings.EncodingName == EncodingDictionary.EncodingMap[0x80].EncodingName)
            return KnownEncodings.ShiftJIS;
        if (knownEncodings.EncodingName == EncodingDictionary.EncodingMap[0x20].EncodingName)
            return KnownEncodings.ASCII;
        if (knownEncodings.EncodingName == EncodingDictionary.EncodingMap[0x40].EncodingName)
            return KnownEncodings.ISO_8859_1;
        if (knownEncodings.EncodingName == EncodingDictionary.EncodingMap[0x60].EncodingName)
            return KnownEncodings.EUC_JP;
        if (knownEncodings.EncodingName == EncodingDictionary.EncodingMap[0xA0].EncodingName)
            return KnownEncodings.UTF8;
        throw new ArgumentOutOfRangeException(nameof(knownEncodings), knownEncodings, null);
    }
}