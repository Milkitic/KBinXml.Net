using System;
using KbinXml.Utils;

namespace KbinXml.Internal;

internal delegate int WriteStringDelegate(ValueListBuilder<byte> builder, ReadOnlySpan<char> str);
internal delegate string ByteToStringDelegate(ReadOnlySpan<byte> bytes);