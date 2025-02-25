#if DEBUG

#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#endif
using System.Collections.Generic;
using System.Linq;

namespace KbinXml.Net.Utils;

internal class DebugUtils
{
    private static readonly IReadOnlyDictionary<int, string> NonPrintDict = CreateNonPrintDict()
#if NET8_0_OR_GREATER
            .ToFrozenDictionary()
#endif
        ;

    public static string GetDisplayableString(string value)
    {
        var arr = value.SelectMany(c =>
            char.IsControl(c) ? GetDisplayable(c) : c.ToString());
        return new string(arr.ToArray());
    }

    public static string GetDisplayable(char c)
    {
        return NonPrintDict.TryGetValue(c, out var value)
            ? $"[\\{value}]"
            : $"[\\{(int)c}]";
    }

    private static Dictionary<int, string> CreateNonPrintDict() => new()
    {
        [00] = "NULL",
        [01] = "SOH",
        [02] = "STX",
        [03] = "ETX",
        [04] = "EOT",
        [05] = "ENQ",
        [06] = "ACK",
        [07] = "BEL",
        [08] = "BS",
        [09] = "HT",
        [10] = "LF",
        [11] = "VT",
        [12] = "FF",
        [13] = "CR",
        [14] = "SO",
        [15] = "SI",
        [16] = "DLE",
        [17] = "DC1",
        [18] = "DC2",
        [19] = "DC3",
        [20] = "DC4",
        [21] = "NAK",
        [22] = "SYN",
        [23] = "ETB",
        [24] = "CAN",
        [25] = "EM",
        [26] = "SUB",
        [27] = "ESC",
        [28] = "FS",
        [29] = "GS",
        [30] = "RS",
        [31] = "US",
        [127] = "DEL",
    };
}
#endif