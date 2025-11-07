using System;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Internal.Writers;

internal ref partial struct WriteContextManager
{
    public void Write16(scoped ReadOnlySpan<byte> span)
    {
        var position = _tracker.Pos16;

        _currentWriteSize = span.Length;
        var streamPositionShift = ComputePositionShift(position);

        if (streamPositionShift == 0)
        {
            AdvancePos32ToNextAlignment(position);
            Write16CoreFast(span);
            return;
        }

        if (streamPositionShift < 0)
        {
            Write16CoreFast(span, position);
            return;
        }

        AdvancePos32ToNextAlignment(position);
        Write16CoreSlow(span, streamPositionShift, _currentWriteSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> BeginWrite16(int size)
    {
        var position = _tracker.Pos16;

        _currentWriteSize = size;
        _streamPositionShift = ComputePositionShift(position);
        if (_streamPositionShift < 0)
        {
            return AllocateWriteBufferWithSeek(position);
        }

        AdvancePos32ToNextAlignment(position);
        return AllocateWriteBuffer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndWrite16()
    {
        _stream.Advance(_advanceHint);
        if (_streamPositionShift < 0) _stream.Position = _savedStreamPosition;
        FinalizeWrite8Or16(ref _tracker.Pos16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write16CoreFast(scoped ReadOnlySpan<byte> span)
    {
        _stream.Write(span);
        FinalizeWrite8Or16(ref _tracker.Pos16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write16CoreFast(scoped ReadOnlySpan<byte> span, int position)
    {
        _savedStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.Write(span);
        _stream.Position = _savedStreamPosition;

        FinalizeWrite8Or16(ref _tracker.Pos16);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write16CoreSlow(scoped ReadOnlySpan<byte> span, int streamPositionShift, int currentWriteSize)
    {
        var stream = _stream;
        var buffer = AllocateWriteBufferWithGap(stream, streamPositionShift, currentWriteSize, out var advanceHint);
        span.CopyTo(buffer);
        stream.Advance(advanceHint);

        FinalizeWrite8Or16(ref _tracker.Pos16);
    }
}