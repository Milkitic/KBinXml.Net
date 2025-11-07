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
    public int Deserialize(Span<byte> span, ReadOnlySpan<char> str)
    {
        return Converter.Deserialize(span, str);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int Deserialize(ref ValueListBuilder<byte> builder, ReadOnlySpan<char> str)
    {
        return Converter.Deserialize(ref builder, str);
    }

    public int Deserialize(ref ValueListBuilder<byte> builder,
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

                var add = Converter.Deserialize(ref builder, s);
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
    public string Serialize(ReadOnlySpan<byte> bytes)
    {
        return Converter.Serialize(bytes);
    }

#if NET8_0_OR_GREATER
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SerializeAppend(ref ValueStringBuilder stringBuilder, ReadOnlySpan<byte> span)
    {
        Converter.SerializeAppend(ref stringBuilder, span);
    }
#endif
}