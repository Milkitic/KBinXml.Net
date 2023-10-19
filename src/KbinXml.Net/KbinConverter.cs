using System;
using System.Collections.Generic;
using System.Linq;
using KbinXml.Net.Internal;

namespace KbinXml.Net;

/// <summary>
/// Provides functionality to convert XML to KBin and to convert KBin into XML text, bytes or XElement.
/// </summary>
public static partial class KbinConverter
{
    private static readonly Type ControlTypeT = typeof(ControlType);
    private static readonly HashSet<byte> ControlTypes =
#if NET5_0_OR_GREATER
        new(Enum.GetValues<ControlType>().Cast<byte>());
#else
        new(Enum.GetValues(ControlTypeT).Cast<byte>());
#endif

    internal static string GetActualName(string name, string? repairedPrefix)
    {
        if (repairedPrefix is not null && name.StartsWith(repairedPrefix, StringComparison.Ordinal))
        {
            return name.Substring(repairedPrefix.Length);
        }
        else
        {
            return name;
        }
    }

    internal static string GetRepairedName(string name, string? repairedPrefix)
    {
        if (repairedPrefix is null) return name;
        if (name.Length < 1 || name[0] < 48 || name[0] > 57) return name;

        return repairedPrefix + name;
    }
}