using System;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal;

internal delegate int WriteStringDelegate(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str);
internal delegate string ByteToStringDelegate(ReadOnlySpan<byte> bytes);
