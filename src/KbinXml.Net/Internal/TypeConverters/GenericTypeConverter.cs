#if NET8_0_OR_GREATER
using System;
using System.Globalization;
using System.Numerics;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class GenericTypeConverter<T> : ITypeConverter
    where T : IBinaryInteger<T>, IMinMaxValue<T>
{
    private GenericTypeConverter() { }

    public static GenericTypeConverter<T> Instance { get; } = new();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(Span<byte> span, ReadOnlySpan<char> str)
    {
        var style = ConvertHelper.GetNumberStyle(str, out str);
        var value = T.Parse(str, style, CultureInfo.InvariantCulture);
        return BitConverterHelper.WriteBeBytes(span, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        var style = ConvertHelper.GetNumberStyle(str, out str);
        var value = T.Parse(str, style, CultureInfo.InvariantCulture);
        return BitConverterHelper.WriteBeBytes(ref builder, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string Serialize(ReadOnlySpan<byte> span)
    {
        return BitConverterHelper.ToBe<T>(span).ToString()!;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeAppend(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        stringBuilder.AppendSpanFormattable(BitConverterHelper.ToBe<T>(span));
    }
}
#endif
