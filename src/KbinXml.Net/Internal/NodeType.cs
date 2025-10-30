using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal;

internal class NodeType
{
    public readonly int Size;
    public readonly int Count;
    public readonly string Name;
    public readonly ITypeConverter Converter;

    public NodeType(int size, int count, string name, ITypeConverter converter)
    {
        Size = size;
        Count = count;
        Name = name;
        Converter = converter;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(Span<byte> span, ReadOnlySpan<char> str)
    {
        return Converter.WriteString(span, str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int WriteString(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        return Converter.WriteString(ref builder, str);
    }

    public int WriteStrings(ref ValueListBuilder<byte> builder,
        ReadOnlySpan<char> arrayCountSpan,
        ReadOnlySpan<char> pendingValueSpan,
        int requiredBytes, bool strictMode)
    {
        int bytesWritten = 0;
        var valueEnumerator = pendingValueSpan.SpanSplit(' ');
        foreach (var s in valueEnumerator)
        {
            try
            {
                if (bytesWritten == requiredBytes)
                {
                    if (strictMode)
                    {
                        throw new KbinArrayCountMissMatchException(arrayCountSpan.ToString(),
                            pendingValueSpan.ToString().Split(' ').Length);
                    }

                    break;
                }

                var add = Converter.WriteString(ref builder, s);
                if (add < Size)
                {
                    builder.AppendZeros(Size - add);
                }

                bytesWritten += Size;
            }
            catch (Exception e)
            {
                throw new KbinException(
                    $"Error while writing data '{s.ToString()}'. See InnerException for more information.",
                    e);
            }
        }

        return bytesWritten;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string GetString(ReadOnlySpan<byte> bytes)
    {
        return Converter.ToString(bytes);
    }

#if NET6_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AppendString(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        Converter.AppendString(ref stringBuilder, span);
    }
#endif
}