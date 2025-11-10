using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class U8Converter : ITypeConverter
{
    private U8Converter()
    {
    }

    public static U8Converter Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(Span<byte> span, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        span[0] = ParseHelper.ParseByte(str, numberStyle);
        return 1; // 写入 1 个字节
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        builder.Append(ParseHelper.ParseByte(str, numberStyle));
        return 1; // 写入 1 个字节
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Serialize(ReadOnlySpan<byte> span)
    {
        return span[0].ToString();
    }

#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeAppend(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(span[0]);
    }
#endif
}