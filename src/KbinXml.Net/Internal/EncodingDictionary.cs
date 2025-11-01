using System.Collections.Generic;
using System.Linq;
using System.Text;

#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif

namespace KbinXml.Net.Internal;

internal static class EncodingDictionary
{
    internal static readonly Encoding EncodingLatin1;
    internal static readonly Encoding EncodingEucJp;
    internal static readonly Encoding EncodingShiftJis;

    static EncodingDictionary()
    {
#if NET8_0_OR_GREATER
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        EncodingLatin1 = Encoding.Latin1;
#else
        EncodingLatin1 = Encoding.GetEncoding("ISO-8859-1");
#endif
        EncodingEucJp = Encoding.GetEncoding("EUC-JP");
        EncodingShiftJis = Encoding.GetEncoding("SHIFT-JIS");
        EncodingMap = new Dictionary<byte, Encoding>
                {
                    { 0x00, EncodingLatin1 },
                    { 0x20, Encoding.ASCII },
                    { 0x40, EncodingLatin1 },
                    { 0x60, EncodingEucJp },
                    { 0x80, EncodingShiftJis },
                    { 0xA0, Encoding.UTF8 },
                }
#if NET8_0_OR_GREATER
                .ToFrozenDictionary()
#endif
            ;
        ReverseEncodingMap = EncodingMap
                .Skip(1)
#if NET8_0_OR_GREATER
                .ToFrozenDictionary(x => x.Value, x => x.Key)
#else
                .ToDictionary(x => x.Value, x => x.Key)
#endif
            ;
    }

    internal static readonly IReadOnlyDictionary<byte, Encoding> EncodingMap;

    internal static readonly IReadOnlyDictionary<Encoding, byte> ReverseEncodingMap;
}