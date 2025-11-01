using System;
using System.Runtime.CompilerServices;
using System.Text;

namespace KbinXml.Net.Internal.Readers;

internal ref partial struct NodeReader : IKBinReader
{
    private const int KbinLengthMask = 0xBF;

    private readonly ReadOnlySpan<byte> _span;
    private readonly Encoding _encoding;
    private readonly bool _compressed;

    private int _position;

    public NodeReader(ReadOnlySpan<byte> span, Encoding encoding, bool compressed)
    {
        _span = span;
        _compressed = compressed;
        _encoding = encoding;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueReadResult<string> ReadString()
    {
        var result = ReadU8();
        var length = result.Value;
        return _compressed
            ? ReadCompressedString(length)
            : ReadUncompressedString(length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ValueReadResult<string> ReadCompressedString(byte length)
    {
        var byteCount = (length * 6 + 7) / 8;
        //var spanResult = ReadBytes((int)Math.Ceiling(length * 6 / 8.0));
        var spanResult = ReadBytes(byteCount);
        var readString = SixbitHelper.Decode(spanResult.Span, length);
        return new ValueReadResult<string>
        (
            readString
#if USELOG
            , spanResult.ReadStatus
#endif
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private unsafe ValueReadResult<string> ReadUncompressedString(byte length)
    {
        var readSpanResult = ReadBytes((length & KbinLengthMask) + 1);

#if NET8_0_OR_GREATER
        return new ValueReadResult<string>
        (
            _encoding.GetString(readSpanResult.Span)
#if USELOG
            , readSpanResult.ReadStatus
#endif
        );
#else
        fixed (byte* p = readSpanResult.Span)
        {
            return new ValueReadResult<string>
            (
                _encoding.GetString(p, readSpanResult.Span.Length)
#if USELOG
                , readSpanResult.ReadStatus
#endif
            );
        }
#endif
    }
}