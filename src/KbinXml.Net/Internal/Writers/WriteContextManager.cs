using System;
using System.Diagnostics;
using Microsoft.IO;

namespace KbinXml.Net.Internal.Writers;

internal ref partial struct WriteContextManager
{
    private readonly RecyclableMemoryStream _stream;
    private DataPositionTracker _tracker;

    private int _currentWriteSize;
    private int _streamPositionShift;

    private long _savedStreamPosition;
    private int _advanceHint;

    public WriteContextManager(RecyclableMemoryStream stream)
    {
        _stream = stream;
    }

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

    private Span<byte> AllocateWriteBufferWithGap()
    {
        _advanceHint = _streamPositionShift + _currentWriteSize;
        var span = _stream.GetSpan(_advanceHint);
        ZeroFillGap(span, _streamPositionShift);
        return span.Slice(_streamPositionShift); // 外部按需进行size切片，提升性能
    }

    /// <summary>
    /// Slow path buffer allocation with stream seek
    /// </summary>
    /// <param name="position"></param>
    /// <returns></returns>
    private Span<byte> AllocateWriteBufferWithSeek(int position)
    {
        _savedStreamPosition = _stream.Position;
        _advanceHint = _currentWriteSize;
        _stream.Position = position;
        var span = _stream.GetSpan(_currentWriteSize);
        return span; // 外部按需进行size切片，提升性能
    }

    private void FinalizeWrite8Or16(ref int pointer)
    {
        pointer += _currentWriteSize;
        _tracker.Realign16_8();
    }

    private void FinalizeWrite32()
    {
        _tracker.Pos32 += _currentWriteSize;
        _tracker.Align32();
    }

    private int ComputePositionShift(int pointer)
    {
        return pointer - (int)_stream.Length;
    }

    private void AdvancePos32ToNextAlignment(int position)
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

    private static void ZeroFillGap(Span<byte> span, int clearLength)
    {
        if (clearLength > 0)
        {
            span.Slice(0, clearLength).Clear();
        }
    }
}