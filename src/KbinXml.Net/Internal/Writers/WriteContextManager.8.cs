using System;
using System.Runtime.CompilerServices;

namespace KbinXml.Net.Internal.Writers;

internal ref partial struct WriteContextManager
{
    public void Write8(byte value)
    {
        var position = _tracker.Pos8;

        _currentWriteSize = 1;
        var streamPositionShift = ComputePositionShift(position);

        if (streamPositionShift == 0)
        {
            AdvancePos32IfAligned(position);
            Write8CoreAppend(value);
            return;
        }

        if (streamPositionShift < 0)
        {
            Write8CoreAt(value, position);
            return;
        }

        AdvancePos32IfAligned(position);
        Write8CoreWithGap(value, streamPositionShift, _currentWriteSize);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> BeginWrite8(int size)
    {
        var position = _tracker.Pos8;

        _currentWriteSize = size;
        _streamPositionShift = ComputePositionShift(position);
        if (_streamPositionShift < 0)
        {
            return AllocateWriteBufferWithSeek(position);
        }

        AdvancePos32IfAligned(position);
        return AllocateWriteBuffer();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndWrite8()
    {
        _stream.Advance(_advanceHint);
        if (_streamPositionShift < 0) _stream.Position = _savedStreamPosition;
        FinalizeWrite8Or16(ref _tracker.Pos8);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write8CoreAppend(byte value)
    {
        _stream.WriteByte(value);
        FinalizeWrite8Or16(ref _tracker.Pos8);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write8CoreAt(byte value, int position)
    {
        _savedStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.WriteByte(value);
        _stream.Position = _savedStreamPosition;

        FinalizeWrite8Or16(ref _tracker.Pos8);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Write8CoreWithGap(byte value, int streamPositionShift, int currentWriteSize)
    {
        var stream = _stream;
        var buffer = AllocateWriteBufferWithGap(stream, streamPositionShift, currentWriteSize, out var advanceHint);
        buffer[0] = value;
        stream.Advance(advanceHint);

        FinalizeWrite8Or16(ref _tracker.Pos8);
    }
}