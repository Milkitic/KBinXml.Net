using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class S32Converter : ITypeConverter
{
    private S32Converter()
    {
    }

    public static S32Converter Instance { get; } = new();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(Span<byte> span, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(span, ParseHelper.ParseInt32(str));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseInt32(str));
        // 返回 4（大端字节序写入 4 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(ReadOnlySpan<byte> span)
    {
        return BitConverterHelper.ToBeInt32(span).ToString();
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendString(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(BitConverterHelper.ToBeInt32(span));
    }
#endif
}