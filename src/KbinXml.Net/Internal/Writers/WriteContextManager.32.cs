using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.InteropServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Writers;

internal ref partial struct WriteContextManager
{
    public void Write(uint value)
    {
        var position = _tracker.Pos32;

        _currentWriteSize = sizeof(uint);
        _streamPositionShift = ComputePositionShift(position);

#if !NETSTANDARD2_0
        if (_streamPositionShift <= 0)
        {
            if (BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            var span = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));
            if (_streamPositionShift == 0)
                FastWrite32(span);
            else
                FastWrite32(span, position);
            return;
        }
#endif

        var buffer = AllocateWriteBuffer();
        BitConverterHelper.WriteBeBytes(buffer, value);
        EndWrite32();
    }

    public void Write32(scoped ReadOnlySpan<byte> span)
    {
        var position = _tracker.Pos32;

        _currentWriteSize = span.Length;
        _streamPositionShift = ComputePositionShift(position);

        if (_streamPositionShift == 0)
        {
            FastWrite32(span);
            return;
        }

        if (_streamPositionShift < 0)
        {
            FastWrite32(span, position);
            return;
        }

        var buffer = AllocateWriteBuffer();
        span.CopyTo(buffer);
        EndWrite32();
    }

    public Span<byte> BeginWrite32(int size, bool trustPosition = false)
    {
        var position = _tracker.Pos32;

        _currentWriteSize = size;
        if (trustPosition)
        {
            _streamPositionShift = 0;
            return AllocateWriteBuffer();
        }

        _streamPositionShift = ComputePositionShift(position);
        if (_streamPositionShift >= 0)
        {
            return AllocateWriteBuffer();
        }

        Debug.Assert(false);
        return AllocateWriteBufferWithSeek(position);
    }

    public void EndWrite32()
    {
        _stream.Advance(_advanceHint);
        Debug.Assert(_streamPositionShift >= 0);
        //if (_positionShift < 0) _stream.Position = _originalStreamPosition;
        FinalizeWrite32();
    }

    private void FastWrite32(scoped ReadOnlySpan<byte> span)
    {
        _stream.Write(span);
        FinalizeWrite32();
    }

    private void FastWrite32(scoped ReadOnlySpan<byte> span, int position)
    {
        _savedStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.Write(span);
        _stream.Position = _savedStreamPosition;

        FinalizeWrite32();
    }
}