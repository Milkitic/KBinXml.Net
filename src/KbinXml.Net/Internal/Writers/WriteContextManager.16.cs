using System;

namespace KbinXml.Net.Internal.Writers;

internal ref partial struct WriteContextManager
{
    public void Write16(scoped ReadOnlySpan<byte> span)
    {
        var position = _tracker.Pos16;

        _currentWriteSize = span.Length;
        _streamPositionShift = ComputePositionShift(position);

        if (_streamPositionShift == 0)
        {
            AdvancePos32ToNextAlignment(position);
            FastWrite16(span);
            return;
        }

        if (_streamPositionShift < 0)
        {
            FastWrite16(span, position);
            return;
        }

        AdvancePos32ToNextAlignment(position);
        var buffer = AllocateWriteBuffer();
        span.CopyTo(buffer);
        EndWrite16();
    }

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

    public void EndWrite16()
    {
        _stream.Advance(_advanceHint);
        if (_streamPositionShift < 0) _stream.Position = _savedStreamPosition;
        FinalizeWrite8Or16(ref _tracker.Pos16);
    }

    private void FastWrite16(scoped ReadOnlySpan<byte> span)
    {
        _stream.Write(span);
        FinalizeWrite8Or16(ref _tracker.Pos16);
    }

    private void FastWrite16(scoped ReadOnlySpan<byte> span, int position)
    {
        _savedStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.Write(span);
        _stream.Position = _savedStreamPosition;

        FinalizeWrite8Or16(ref _tracker.Pos16);
    }
}