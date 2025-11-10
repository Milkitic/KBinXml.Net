using System;
using System.Runtime.CompilerServices;

namespace UnitBenchmarks.Legacy;

internal ref partial struct WriteContextManagerV1_5
{
    public void Write16(scoped ReadOnlySpan<byte> span)
    {
        var position = _tracker.Pos16;

        _currentWriteSize = span.Length;
        var streamPositionShift = ComputePositionShift(position);

        if (streamPositionShift == 0)
        {
            AdvancePos32IfAligned(position);
            WriteCoreAppend(span);
        }
        else if (streamPositionShift < 0)
        {
            WriteCoreAt(span, position);
        }
        else
        {
            AdvancePos32IfAligned(position);
            WriteCoreWithGap(span, streamPositionShift, _currentWriteSize);
        }

        FinalizeWrite8Or16(ref _tracker.Pos16);
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

        AdvancePos32IfAligned(position);
        return AllocateWriteBuffer();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndWrite16()
    {
        _stream.Advance(_advanceHint);
        if (_streamPositionShift < 0) _stream.Position = _savedStreamPosition;
        FinalizeWrite8Or16(ref _tracker.Pos16);
    }
}