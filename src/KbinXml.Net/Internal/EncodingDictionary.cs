using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace KbinXml.Net.Internal;

internal static class EncodingDictionary
{
    static EncodingDictionary()
    {
#if NETCOREAPP3_1_OR_GREATER
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        EncodingMap = new Dictionary<byte, Encoding>
            {
                { 0x00, Encoding.GetEncoding("ISO-8859-1") },
                { 0x20, Encoding.ASCII },
                { 0x40, Encoding.GetEncoding("ISO-8859-1") },
                { 0x60, Encoding.GetEncoding("EUC-JP") },
                { 0x80, Encoding.GetEncoding("SHIFT-JIS") },
                { 0xA0, Encoding.UTF8 },
            }
#if NET8_0_OR_GREATER
            .ToFrozenDictionary()
#endif
            ;
        ReverseEncodingMap = EncodingMap
                .Skip(1)
                .ToDictionary(x => x.Value, x => x.Key)
#if NET8_0_OR_GREATER
                .ToFrozenDictionary()
#endif
            ;
    }

    internal static readonly IReadOnlyDictionary<byte, Encoding> EncodingMap;

    internal static readonly IReadOnlyDictionary<Encoding, byte> ReverseEncodingMap;
}