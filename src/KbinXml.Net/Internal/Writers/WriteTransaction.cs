using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Microsoft.IO;

namespace KbinXml.Net.Internal.Writers;

internal unsafe ref struct WriteTransaction : IDisposable
{
    private bool _isAcquired;
    private long _originalPosition;
    private readonly RecyclableMemoryStream _stream;
    private readonly int _writeOffset;
    private readonly int _size;
    private readonly AlignmentGroup _alignmentGroup;

    private readonly DataPositionTracker* _tracker;
    private readonly int* _activePointer;

    public WriteTransaction(RecyclableMemoryStream stream, scoped ref DataPositionTracker tracker,
        AlignmentGroup alignmentGroup, int size)
    {
        _stream = stream;
        _size = size;
        _alignmentGroup = alignmentGroup;

        fixed (DataPositionTracker* p = &tracker)
        {
            _tracker = p;
        }

        switch (alignmentGroup)
        {
            case AlignmentGroup.Align8:
                _activePointer = &(_tracker->Pos8);
                break;
            case AlignmentGroup.Align16:
                _activePointer = &(_tracker->Pos16);
                break;
            case AlignmentGroup.Align32:
            default:
                _activePointer = &(_tracker->Pos32);
                break;
        }

        _writeOffset = CalculateWriteOffset(*_activePointer);
    }

    public Span<byte> Buffer
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _isAcquired ? field : field = AcquireSpan();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int CalculateWriteOffset(int pointer)
    {
        return pointer - (int)_stream.Length;
    }

    private Span<byte> AcquireSpan()
    {
        _isAcquired = true;
        _originalPosition = _stream.Position;

        if (_alignmentGroup != AlignmentGroup.Align32)
        {
            if ((*_activePointer & 3) == 0)
            {
                Debug.Assert(_writeOffset >= 0);
                // 非32位写入且处于4字节边界时，推进32位指针以避免后续重叠
                _tracker->Pos32 += 4;
            }
            else
            {
                Debug.Assert(_writeOffset <= 0);
            }
        }

        if (_writeOffset >= 0)
        {
            var sizeHint = _writeOffset + _size;
            var span = _stream.GetSpan(sizeHint);
            ClearSpan(span, _writeOffset);
            return span.Slice(_writeOffset); // 外部按需进行size切片，提升性能
        }
        else
        {
            _stream.Position = *_activePointer;
            var span = _stream.GetSpan(_size);
            return span; // 外部按需进行size切片，提升性能
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ClearSpan(Span<byte> span, int clearLength)
    {
        if (clearLength > 0)
        {
            span.Slice(0, clearLength).Clear();
        }
    }

    public void Dispose()
    {
        if (!_isAcquired) return;

        if (_writeOffset >= 0)
        {
            var sizeHint = _writeOffset + _size;
            _stream.Advance(sizeHint);
        }
        else
        {
            _stream.Advance(_size);
            _stream.Position = _originalPosition;
        }

        // 提交写入后推进选定指针（8/16/32 位）
        *_activePointer += _size;

        if (_alignmentGroup == AlignmentGroup.Align32)
        {
            _tracker->Align32();
        }
        else
        {
            _tracker->Realign16_8();
        }
    }
}