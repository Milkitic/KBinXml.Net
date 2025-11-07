using System;

namespace KbinXml.Net.Internal.Writers;

internal ref partial struct WriteContextManager
{
    public void Write(byte value)
    {
        var position = _tracker.Pos8;

        _currentWriteSize = 1;
        _streamPositionShift = ComputePositionShift(position);

        if (_streamPositionShift == 0)
        {
            AdvancePos32ToNextAlignment(position);
            FastWrite8(value);
            return;
        }

        if (_streamPositionShift < 0)
        {
            FastWrite8(value, position);
            return;
        }

        AdvancePos32ToNextAlignment(position);
        var buffer = AllocateWriteBuffer();
        buffer[0] = value;
        EndWrite8();
    }

    public Span<byte> BeginWrite8(int size)
    {
        var position = _tracker.Pos8;

        _currentWriteSize = size;
        _streamPositionShift = ComputePositionShift(position);
        if (_streamPositionShift < 0)
        {
            return AllocateWriteBufferWithSeek(position);
        }

        AdvancePos32ToNextAlignment(position);
        return AllocateWriteBuffer();
    }

    public void EndWrite8()
    {
        _stream.Advance(_advanceHint);
        if (_streamPositionShift < 0) _stream.Position = _savedStreamPosition;
        FinalizeWrite8Or16(ref _tracker.Pos8);
    }

    private void FastWrite8(byte value)
    {
        _stream.WriteByte(value);
        FinalizeWrite8Or16(ref _tracker.Pos8);
    }

    private void FastWrite8(byte value, int position)
    {
        _savedStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.WriteByte(value);
        _stream.Position = _savedStreamPosition;

        FinalizeWrite8Or16(ref _tracker.Pos8);
    }
}