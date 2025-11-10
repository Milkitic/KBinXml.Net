using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class FloatConverter : ITypeConverter
{
    private FloatConverter()
    {
    }

    public static FloatConverter Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(Span<byte> span, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(span, ParseHelper.ParseSingle(str, ConvertHelper.UsNumberFormat));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseSingle(str, ConvertHelper.UsNumberFormat));
        // 返回 4（大端字节序写入 4 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Serialize(ReadOnlySpan<byte> span)
    {
        return BitConverterHelper.ToBeSingle(span).ToString("0.000000"); // 保留 6 位小数
    }

#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeAppend(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(BitConverterHelper.ToBeSingle(span), "0.000000");
    }
#endif
}