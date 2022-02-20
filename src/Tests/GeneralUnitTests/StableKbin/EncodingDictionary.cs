using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace StableKbin
{
    internal static class EncodingDictionary
    {
        internal static readonly Dictionary<byte, Encoding> EncodingMap = new Dictionary<byte, Encoding>()
        {
            { 0x00, Encoding.GetEncoding("ISO-8859-1") },
            { 0x20, Encoding.ASCII                     },
            { 0x40, Encoding.GetEncoding("ISO-8859-1") },
            { 0x60, Encoding.GetEncoding("EUC-JP")     },
            { 0x80, Encoding.GetEncoding("SHIFT-JIS")  },
            { 0xA0, Encoding.UTF8                      },
        };

        internal static readonly Dictionary<Encoding, byte> ReverseEncodingMap = EncodingMap.Where(x => x.Key != 0).ToDictionary(x => x.Value, x => x.Key);
    }
}
