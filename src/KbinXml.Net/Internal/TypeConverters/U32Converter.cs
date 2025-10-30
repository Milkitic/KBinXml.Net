using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class U32Converter : ITypeConverter
{
    private U32Converter()
    {
    }

    public static U32Converter Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(Span<byte> span, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(span, ParseHelper.ParseUInt32(str, numberStyle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt32(str, numberStyle));
        // 返回 4（大端字节序写入 4 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(ReadOnlySpan<byte> span)
    {
        return BitConverterHelper.ToBeUInt32(span).ToString();
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendString(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(BitConverterHelper.ToBeUInt32(span));
    }
#endif
}