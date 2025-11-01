using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Writers;

internal partial struct DataWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS8(sbyte value)
    {
        WriteByte((byte)value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU8(byte value)
    {
        WriteByte(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS16(short value)
    {
        Write16BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU16(ushort value)
    {
        Write16BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS32(int value)
    {
        Write32BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU32(uint value)
    {
        Write32BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS64(long value)
    {
        Write32BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU64(ulong value)
    {
        Write32BitAlignedInternal(value);
    }

    private void Write16BitAlignedInternal<T>(T value) where T : unmanaged
    {
        const int size = 2; // sizeof(short) or sizeof(ushort)
        ref var pointer = ref _tracker.Pos16;
        var increment = GetIncrementLength(pointer);

        if ((pointer & 3) == 0) // 如果16位指针是4字节对齐的
        {
            _tracker.Pos32 += 4;
        }

        if (increment >= 0)
        {
            var sizeHint = increment + size;
            var span = Stream.GetSpan(sizeHint);
            if (increment > 0)
            {
                ClearSpan(span, increment);
                BitConverterHelper.WriteBeBytesT(span.Slice(increment), value);
            }
            else
            {
                BitConverterHelper.WriteBeBytesT(span, value);
            }

            Stream.Advance(sizeHint);
        }
        else
        {
            var streamPosition = Stream.Position;
            Stream.Position = pointer;

            var span = Stream.GetSpan(size);
            BitConverterHelper.WriteBeBytesT(span, value);
            Stream.Advance(size);

            Stream.Position = streamPosition;
        }

        pointer += size;
        _tracker.Realign16_8();
    }

    private void Write32BitAlignedInternal<T>(T value) where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        ref var pointer = ref _tracker.Pos32;
        var increment = GetIncrementLength(pointer);

        if (increment >= 0)
        {
            var sizeHint = increment + size;
            var span = Stream.GetSpan(sizeHint);
            if (increment > 0)
            {
                ClearSpan(span, increment);
                BitConverterHelper.WriteBeBytesT(span.Slice(increment), value);
            }
            else
            {
                BitConverterHelper.WriteBeBytesT(span, value);
            }

            Stream.Advance(sizeHint);
        }
        else
        {
            var streamPosition = Stream.Position;
            Stream.Position = pointer;

            var span = Stream.GetSpan(size);
            BitConverterHelper.WriteBeBytesT(span, value);
            Stream.Advance(size);

            Stream.Position = streamPosition;
        }

        pointer += size;
        _tracker.Align32();
    }
}