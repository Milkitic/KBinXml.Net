using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.TypeConverters;

internal sealed class DummyBinConverter : ITypeConverter
{
    private DummyBinConverter()
    {
    }

    public static DummyBinConverter Instance { get; } = new();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(Span<byte> span, ReadOnlySpan<char> str)
    {
        throw new NotSupportedException("Binary data should not be written as string.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        throw new NotSupportedException("Binary data should not be written as string.");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ToString(ReadOnlySpan<byte> span)
    {
        throw new NotSupportedException("Binary data should not be converted to string.");
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendString(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        throw new NotSupportedException("Binary data should not be converted to string.");
    }
#endif
}