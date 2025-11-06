using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.IO;

namespace KbinXml.Net.Internal.Writers;

internal ref struct WriteContextManager
{
    private readonly RecyclableMemoryStream _stream;
    private DataPositionTracker _tracker;

    private int _currentSize;
    private int _positionShift;

    private long _originalStreamPosition;
    private int _sizeHint;

    public WriteContextManager(RecyclableMemoryStream stream)
    {
        _stream = stream;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8(byte value)
    {
        var position = _tracker.Pos8;

        _currentSize = 1;
        _positionShift = CalculatePositionShift(position);
        Move32ToNextAlign(position);

        if (_positionShift < 0)
        {
            EndWrite8Fast(value, position);
            return;
        }

        var buffer = BeginWrite(position);
        buffer[0] = value;
        EndWrite8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write16(scoped ReadOnlySpan<byte> span)
    {
        var position = _tracker.Pos8;

        _currentSize = span.Length;
        _positionShift = CalculatePositionShift(position);
        Move32ToNextAlign(position);

        if (_positionShift < 0)
        {
            EndWrite16Fast(span, position);
            return;
        }

        var buffer = BeginWrite(position);
        span.CopyTo(buffer);
        EndWrite16();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> BeginWrite8(int size)
    {
        var position = _tracker.Pos8;

        _currentSize = size;
        _positionShift = CalculatePositionShift(position);
        Move32ToNextAlign(position);
        return BeginWrite(position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> BeginWrite16(int size)
    {
        var position = _tracker.Pos16;

        _currentSize = size;
        _positionShift = CalculatePositionShift(position);
        Move32ToNextAlign(position);
        return BeginWrite(position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<byte> BeginWrite32(int size)
    {
        var position = _tracker.Pos32;

        _currentSize = size;
        _positionShift = CalculatePositionShift(position);
        return BeginWrite(position);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndWrite8()
    {
        _stream.Advance(_sizeHint);
        if (_positionShift < 0) _stream.Position = _originalStreamPosition;
        Finalize8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndWrite16()
    {
        _stream.Advance(_sizeHint);
        if (_positionShift < 0) _stream.Position = _originalStreamPosition;
        Finalize16();
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EndWrite32()
    {
        _stream.Advance(_sizeHint);
        Debug.Assert(_positionShift >= 0);
        //if (_positionShift < 0) _stream.Position = _originalStreamPosition;
        _tracker.Pos32 += _currentSize;
        _tracker.Align32();
    }

    private void EndWrite8Fast(byte value, int position)
    {
        _originalStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.WriteByte(value);
        _stream.Position = _originalStreamPosition;

        Finalize8();
    }

    private void EndWrite16Fast(scoped ReadOnlySpan<byte> span, int position)
    {
        _originalStreamPosition = _stream.Position;
        _stream.Position = position;
        _stream.Write(span);
        _stream.Position = _originalStreamPosition;

        Finalize16();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Finalize8()
    {
        _tracker.Pos8 += _currentSize;
        _tracker.Realign16_8();
    }
    private void Finalize16()
    {
        _tracker.Pos16 += _currentSize;
        _tracker.Realign16_8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculatePositionShift(int pointer)
    {
        return pointer - (int)_stream.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Move32ToNextAlign(int position)
    {
        if ((position & 3) == 0)
        {
            Debug.Assert(_positionShift >= 0);
            // 非32位写入且处于4字节边界时，推进32位指针以避免后续重叠
            _tracker.Pos32 += 4;
        }
        else
        {
            Debug.Assert(_positionShift <= 0);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private Span<byte> BeginWrite(int position)
    {
        if (_positionShift == 0)
        {
            _sizeHint = _currentSize;
            return _stream.GetSpan(_currentSize); // 外部按需进行size切片，提升性能
        }

        if (_positionShift >= 0)
        {
            _sizeHint = _positionShift + _currentSize;
            var span = _stream.GetSpan(_sizeHint);
            ClearSpan(span, _positionShift);
            return span.Slice(_positionShift); // 外部按需进行size切片，提升性能
        }

        // Warn: Slow path
        return BeginWriteSlow(position);
    }

    private Span<byte> BeginWriteSlow(int position)
    {
        _originalStreamPosition = _stream.Position;
        _sizeHint = _currentSize;
        _stream.Position = position;
        var span = _stream.GetSpan(_currentSize);
        return span; // 外部按需进行size切片，提升性能
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ClearSpan(Span<byte> span, int clearLength)
    {
        if (clearLength > 0)
        {
            span.Slice(0, clearLength).Clear();
        }
    }
}