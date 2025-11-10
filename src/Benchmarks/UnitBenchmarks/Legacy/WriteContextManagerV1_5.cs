using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using KbinXml.Net.Internal;
using Microsoft.IO;

namespace UnitBenchmarks.Legacy;

internal ref partial struct WriteContextManagerV1_5
{
    private readonly RecyclableMemoryStream _stream;
    private DataPositionTracker _tracker;

    private int _currentWriteSize;
    private int _streamPositionShift;

    private long _savedStreamPosition;
    private int _advanceHint;

    public WriteContextManagerV1_5(RecyclableMemoryStream stream)
    {
        _stream = stream;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCoreAppend(scoped ReadOnlySpan<byte> span)
    {
        _stream.Write(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCoreAppend(byte value)
    {
        _stream.WriteByte(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCoreAt(scoped ReadOnlySpan<byte> span, int position)
    {
        _savedStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.Write(span);
        _stream.Position = _savedStreamPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCoreAt(byte value, int position)
    {
        _savedStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.WriteByte(value);
        _stream.Position = _savedStreamPosition;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCoreWithGap(scoped ReadOnlySpan<byte> span, int streamPositionShift, int currentWriteSize)
    {
        var stream = _stream;
        var buffer = AllocateWriteBufferWithGap(stream, streamPositionShift, currentWriteSize, out var advanceHint);
        span.CopyTo(buffer);
        stream.Advance(advanceHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteCoreWithGap(byte value, int streamPositionShift, int currentWriteSize)
    {
        var stream = _stream;
        var buffer = AllocateWriteBufferWithGap(stream, streamPositionShift, currentWriteSize, out var advanceHint);
        buffer[0] = value;
        stream.Advance(advanceHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<byte> AllocateWriteBuffer()
    {
        Debug.Assert(_streamPositionShift >= 0);

        if (_streamPositionShift == 0)
        {
            _advanceHint = _currentWriteSize;
            return _stream.GetSpan(_currentWriteSize); // 外部按需进行size切片，提升性能
        }

        return AllocateWriteBufferWithGap();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<byte> AllocateWriteBufferWithGap()
    {
        return AllocateWriteBufferWithGap(_stream, _streamPositionShift, _currentWriteSize, out _advanceHint);
    }

    /// <summary>
    /// Slow path buffer allocation with stream seek
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<byte> AllocateWriteBufferWithSeek(int position)
    {
        _savedStreamPosition = _stream.Position;
        _advanceHint = _currentWriteSize;
        _stream.Position = position;
        var span = _stream.GetSpan(_currentWriteSize);
        return span; // 外部按需进行size切片，提升性能
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FinalizeWrite8Or16(ref int pointer)
    {
        pointer += _currentWriteSize;
        _tracker.Realign16_8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void FinalizeWrite32()
    {
        _tracker.Pos32 += _currentWriteSize;
        _tracker.Align32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int ComputePositionShift(int pointer)
    {
        return pointer - (int)_stream.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void AdvancePos32IfAligned(int position)
    {
        if ((position & 3) == 0)
        {
            Debug.Assert(_streamPositionShift >= 0);
            // 非32位写入且处于4字节边界时，推进32位指针以避免后续重叠
            _tracker.Pos32 += 4;
        }
        else
        {
            Debug.Assert(_streamPositionShift <= 0);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static Span<byte> AllocateWriteBufferWithGap(RecyclableMemoryStream stream, int streamPositionShift, int currentWriteSize, out int advanceHint)
    {
        advanceHint = streamPositionShift + currentWriteSize;
        var span = stream.GetSpan(advanceHint);
        span.Slice(0, streamPositionShift).Clear();
        return span.Slice(streamPositionShift); // 外部按需进行size切片，提升性能
    }
}