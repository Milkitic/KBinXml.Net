using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

#if NET8_0_OR_GREATER
internal static class U16Converter
{
    public static ITypeConverter Instance => GenericTypeConverter<ushort>.Instance;
}
#else
internal sealed class U16Converter : ITypeConverter
{
    private U16Converter()
    {
    }

    public static U16Converter Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(Span<byte> span, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(span, ParseHelper.ParseUInt16(str, numberStyle));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var numberStyle = ConvertHelper.GetNumberStyle(str, out str);
        return BitConverterHelper.WriteBeBytes(ref builder, ParseHelper.ParseUInt16(str, numberStyle));
        // 返回 2（大端字节序写入 2 个字节）
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Serialize(ReadOnlySpan<byte> span)
    {
        return BitConverterHelper.ToBeUInt16(span).ToString();
    }

#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeAppend(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(BitConverterHelper.ToBeUInt16(span));
    }
#endif
}
#endif