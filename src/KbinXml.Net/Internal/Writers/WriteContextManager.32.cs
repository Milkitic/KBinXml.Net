using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Writers;

internal ref partial struct WriteContextManager
{
    public void Write32(uint value)
    {
        var position = _tracker.Pos32;

        _currentWriteSize = sizeof(uint);
#if !NETSTANDARD2_0
        var streamPositionShift = ComputePositionShift(position);
        if (streamPositionShift <= 0)
        {
            if (BitConverter.IsLittleEndian)
                value = BinaryPrimitives.ReverseEndianness(value);
            var span = MemoryMarshal.AsBytes(MemoryMarshal.CreateSpan(ref value, 1));
            if (streamPositionShift == 0)
            {
                WriteCoreAppend(span);
            }
            else
            {
#if !NETSTANDARD2_0
                throw new UnreachableException("Write32(value) should not have a negative position shift.");
#endif
                Debug.Assert(false);
                WriteCoreAt(span, position);
            }
        }
        else
        {
            var stream = _stream;
            var buffer = AllocateWriteBufferWithGap(stream, streamPositionShift, _currentWriteSize, out var advanceHint);
            BitConverterHelper.WriteBeBytes(buffer, value);
            stream.Advance(advanceHint);
        }

        FinalizeWrite32();
#else
        _streamPositionShift = ComputePositionShift(position);
        var buffer = AllocateWriteBuffer();
        BitConverterHelper.WriteBeBytes(buffer, value);

        EndWrite32();
#endif
    }

    public void Write32(scoped ReadOnlySpan<byte> span)
    {
        var position = _tracker.Pos32;

        _currentWriteSize = span.Length;
        var streamPositionShift = ComputePositionShift(position);

        if (streamPositionShift == 0)
        {
            WriteCoreAppend(span);
        }
        else if (streamPositionShift > 0)
        {
            WriteCoreWithGap(span, streamPositionShift, _currentWriteSize);
        }
        else
        {
#if !NETSTANDARD2_0
            throw new UnreachableException("Write32(span) should not have a negative position shift.");
#endif
            Debug.Assert(false);
            WriteCoreAt(span, position);
        }

        FinalizeWrite32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> BeginWrite32(int size)
    {
        _currentWriteSize = size;
        var position = _tracker.Pos32;
        _streamPositionShift = ComputePositionShift(position);
        if (_streamPositionShift >= 0)
        {
            return AllocateWriteBuffer();
        }

#if !NETSTANDARD2_0
        throw new UnreachableException("BeginWrite32(size) should not have a negative position shift.");
#endif
        Debug.Assert(false);
        return AllocateWriteBufferWithSeek(position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> BeginWrite32Sequential(int size)
    {
        _currentWriteSize = size;
        _streamPositionShift = 0;
        _advanceHint = _currentWriteSize;
        return _stream.GetSpan(_currentWriteSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndWrite32()
    {
        _stream.Advance(_advanceHint);
        Debug.Assert(_streamPositionShift >= 0);
        //if (_positionShift < 0) _stream.Position = _originalStreamPosition;
        FinalizeWrite32();
    }
}