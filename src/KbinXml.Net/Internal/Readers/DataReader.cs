using System;
using System.Runtime.CompilerServices;
using System.Text;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Readers;

internal ref partial struct DataReader : IKBinReader
{
    private readonly ReadOnlySpan<byte> _span;
    private readonly Encoding _encoding;

    private int _pos;
    private int _pos16;
    private int _pos8;

    public DataReader(ReadOnlySpan<byte> span, Encoding encoding)
    {
        _span = span;
        _encoding = encoding;
    }

    public SpanReadResult ReadBytes(int count)
    {
        return count switch
        {
            1 => ReadBytes8BitAligned(),
            2 => ReadBytes16BitAligned(),
            _ => ReadBytes32BitAligned(count)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanReadResult ReadBytes8BitAligned()
    {
        // Realign before read.
        // If need to, align pos8 to next 4-bytes chunk, and move the generic position to next chunk
        AlignPosition(ref _pos8);

        var span = ReadBytesSafe(_pos8, 1);
        var result = new SpanReadResult
        (
            span
#if USELOG
            , new ReadStatus { Flag = "p8", Offset = _pos8, Length = 1 }
#endif
        );

        _pos8++;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanReadResult ReadBytes16BitAligned()
    {
        // Realign before read.
        // If need to, align pos16 to next 4-bytes chunk, and move the generic position to next chunk
        AlignPosition(ref _pos16);

        var span = ReadBytesSafe(_pos16, 2);
        var result = new SpanReadResult
        (
            span
#if USELOG
            , new ReadStatus { Flag = "p16", Offset = _pos16, Length = 2 }
#endif
        );

        _pos16 += 2;
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public SpanReadResult ReadBytes32BitAligned(int count)
    {
        var span = ReadBytesSafe(_pos, count);
        var result = new SpanReadResult
        (
            span
#if USELOG
            , new ReadStatus { Flag = "p32", Offset = _pos, Length = count }
#endif
        );

        //var left = count & 3;
        //if (left != 0)
        //{
        //    count += (4 - left);
        //}

        //_pos += count;
        _pos += count + 3 & ~3; // 向上取整到4的倍数
        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public unsafe ValueReadResult<string> ReadString(int count)
    {
        var spanResult = ReadBytes32BitAligned(count);
        var span = spanResult.Span.Slice(0, spanResult.Span.Length - 1);
        if (span.Length == 0)
        {
            return new ValueReadResult<string>
            (
                string.Empty
#if USELOG
                , spanResult.ReadStatus
#endif
            );
        }

#if NETCOREAPP3_1_OR_GREATER
        return new ValueReadResult<string>
        (
            _encoding.GetString(span)
#if USELOG
            , spanResult.ReadStatus
#endif
        );
#elif NETSTANDARD2_0 || NET46_OR_GREATER
        fixed (byte* p = span)
        {
            return new ValueReadResult<string>
            (
                _encoding.GetString(p, span.Length)
#if USELOG
                , spanResult.ReadStatus
#endif
            );
        }
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ValueReadResult<string> ReadBinary(int count)
    {
        var spanResult = ReadBytes32BitAligned(count);
        if (spanResult.Span.Length == 0)
        {
            return new ValueReadResult<string>
            (
                string.Empty
#if USELOG
                , spanResult.ReadStatus
#endif
            );
        }

        return new ValueReadResult<string>
        (
            ConvertHelper.ToHexString(spanResult.Span, false)
#if USELOG
            , spanResult.ReadStatus
#endif
        );
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ReadOnlySpan<byte> ReadBytesSafe(int offset, int count)
    {
        int actualCount;
        if (count + offset > _span.Length)
        {
            actualCount = _span.Length - offset;
        }
        else
        {
            actualCount = count;
        }

        var slice = _span.Slice(offset, actualCount);
        return slice;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AlignPosition(ref int alignedPos)
    {
        if ((alignedPos & 3) == 0)
        {
            alignedPos = _pos;
            _pos += 4;
        }
    }
}