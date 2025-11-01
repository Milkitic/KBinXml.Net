using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class U64Converter : ITypeConverter
{
    private U64Converter()
    {
    }

    public static U64Converter Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(Span<byte> span, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(span, ParseHelper.ParseUInt64(str, numberStyle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt64(str, numberStyle));
        // 返回 8（大端字节序写入 8 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(ReadOnlySpan<byte> span)
    {
        return BitConverterHelper.ToBeUInt64(span).ToString();
    }

#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendString(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(BitConverterHelper.ToBeUInt64(span));
    }
#endif
}