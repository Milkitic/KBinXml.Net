using System;

namespace KbinXml.Net.Internal.Readers;

internal ref partial struct BigEndianReader : IKBinReader
{
    private readonly ReadOnlySpan<byte> _span;

    private int _position;

    public BigEndianReader(ReadOnlySpan<byte> span)
    {
        _span = span;
    }

    public SpanReadResult ReadBytes(int count)
    {
        var result = _span.Slice(_position, count);
        var readSpanResult = new SpanReadResult
        (
            result
#if USELOG
            , new ReadStatus { Offset = _position, Length = count }
#endif
        );
        _position += count;
        return readSpanResult;
    }
}