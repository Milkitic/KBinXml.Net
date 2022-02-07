using System;
using System.Text;
using KbinXml.Net.Internal;

namespace KbinXml.Net;

public static class KnownEncodingsExtensions
{
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
}