using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class S16Converter : ITypeConverter
{
    private S16Converter()
    {
    }

    public static S16Converter Instance { get; } = new();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(Span<byte> span, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(span, ParseHelper.ParseInt16(str));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseInt16(str));
        // 返回 2（大端字节序写入 2 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Serialize(ReadOnlySpan<byte> span)
    {
        return BitConverterHelper.ToBeInt16(span).ToString();
    }

#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeAppend(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(BitConverterHelper.ToBeInt16(span));
    }
#endif
}