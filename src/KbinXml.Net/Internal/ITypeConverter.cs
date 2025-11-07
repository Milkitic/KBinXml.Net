using System;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal;

internal interface ITypeConverter
{
    int Deserialize(Span<byte> span, ReadOnlySpan<char> str);
    int Deserialize(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str);
    string Serialize(ReadOnlySpan<byte> span);
#if NET8_0_OR_GREATER
    void SerializeAppend(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span);
#endif
}