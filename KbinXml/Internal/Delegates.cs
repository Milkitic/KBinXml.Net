using System;
using System.IO;

namespace KbinXml.Internal;

internal delegate void WriteStringDelegate(Stream stream,ReadOnlySpan<char> str);
internal delegate string ByteToStringDelegate(ReadOnlySpan<byte> bytes);