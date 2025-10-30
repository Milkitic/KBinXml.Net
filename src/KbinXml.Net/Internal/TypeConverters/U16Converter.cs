using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class U16Converter : ITypeConverter
{
    private U16Converter()
    {
    }

    public static U16Converter Instance { get; } = new();

    public int WriteString(Span<byte> span, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(span, ParseHelper.ParseUInt16(str, numberStyle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt16(str, numberStyle));
        // 返回 2（大端字节序写入 2 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(ReadOnlySpan<byte> span)
    {
        return BitConverterHelper.ToBeUInt16(span).ToString();
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendString(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(BitConverterHelper.ToBeUInt16(span));
    }
#endif
}