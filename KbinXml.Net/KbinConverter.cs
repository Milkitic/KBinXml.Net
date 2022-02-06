using System;
using System.Collections.Generic;
using System.Linq;
using KbinXml.Net.Internal;

namespace KbinXml.Net;

public static partial class KbinConverter
{
    private static readonly Type ControlTypeT = typeof(ControlType);
    private static readonly HashSet<byte> ControlTypes =
#if NET5_0_OR_GREATER
        new(Enum.GetValues<ControlType>().Cast<byte>());
#else
        new(Enum.GetValues(ControlTypeT).Cast<byte>());
#endif
}