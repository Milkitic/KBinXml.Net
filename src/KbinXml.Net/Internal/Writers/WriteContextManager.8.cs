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
            WriteCoreAppend(value);
        }
        else if (streamPositionShift < 0)
        {
            WriteCoreAt(value, position);
        }
        else
        {
            AdvancePos32IfAligned(position);
            WriteCoreWithGap(value, streamPositionShift, _currentWriteSize);
        }

        FinalizeWrite8Or16(ref _tracker.Pos8);
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
}