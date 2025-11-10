using System.Buffers;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using KbinXml.Net;
using KbinXml.Net.Internal;
using KbinXml.Net.Utils;
using Microsoft.IO;

namespace UnitBenchmarks.Legacy;

public ref partial struct DataWriterV1 : IKBinWriter, IDisposable
{
    public readonly RecyclableMemoryStream Stream;
    private readonly Encoding _encoding;
    private readonly bool _disposeStream;

    private DataPositionTracker _tracker;

    public DataWriterV1(Encoding encoding, int capacity = 0)
    {
        _encoding = encoding;
        Stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("wd", capacity);
        _disposeStream = true;
    }

    public DataWriterV1(Encoding encoding, RecyclableMemoryStream stream)
    {
        _encoding = encoding;
        Stream = stream;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte singleByte)
    {
        Write8BitAligned(singleByte);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(scoped ReadOnlySpan<byte> buffer)
    {
        switch (buffer.Length)
        {
            case 1:
                Write8BitAligned(buffer[0]);
                break;

            case 2:
                Write16BitAligned(buffer);
                break;

            default:
                Write32BitAligned(buffer);
                break;
        }
    }

#if NET8_0_OR_GREATER
    public void WriteString(ReadOnlySpan<char> value)
    {
#else
    public void WriteString(ReadOnlySpan<char> span)
    {
        var value = span.ToString();
#endif
        // 计算编码后的字节长度（包括结尾的0字节）
        int byteCount = _encoding.GetByteCount(value) + 1;

        // 先写入长度
        WriteU32((uint)byteCount);

        // 准备写入数据（32位对齐）
        ref var pointer = ref _tracker.Pos32;
        var increment = GetIncrementLength(pointer);

        Debug.Assert(increment == 0); // TODO: 理论上长度写完后已经是对齐的，increment 为 0

        // 获取足够大小的Span并写入数据
        if (increment >= 0)
        {
            WriteStringCore(value, increment, byteCount);
        }
        else
        {
            var streamPosition = Stream.Position;
            Stream.Position = pointer;
            WriteStringCore(value, 0, byteCount);
            Stream.Position = streamPosition;
        }

        pointer += byteCount;
        _tracker.Align32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBinary(ReadOnlySpan<char> value)
    {
        // 计算二进制数据的长度（每两个字符表示一个字节）
        int length = value.Length >> 1;

        // 先写入长度
        WriteU32((uint)length);

        // 准备写入数据（32位对齐）
        ref var pointer = ref _tracker.Pos32;
        var increment = GetIncrementLength(pointer);

        Debug.Assert(increment == 0); // TODO: 理论上长度写完后已经是对齐的，increment 为 0

        // 获取足够大小的Span并写入数据
        if (increment >= 0)
        {
            WriteBinaryCore(value, increment, length);
        }
        else
        {
            var streamPosition = Stream.Position;
            Stream.Position = pointer;
            WriteBinaryCore(value, 0, length);
            Stream.Position = streamPosition;
        }

        pointer += length;
        _tracker.Align32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8BitAligned(byte value)
    {
        var increment = GetIncrementLength(_tracker.Pos8);

        if ((_tracker.Pos8 & 3) == 0)
        {
            Debug.Assert(increment >= 0);
            _tracker.Pos32 += 4;
        }
        else
        {
            Debug.Assert(increment <= 0);
        }

        WriteSingleByte(value, increment, ref _tracker.Pos8);

        _tracker.Realign16_8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write16BitAligned(scoped ReadOnlySpan<byte> buffer)
    {
        var increment = GetIncrementLength(_tracker.Pos16);

        if ((_tracker.Pos16 & 3) == 0)
        {
            Debug.Assert(increment >= 0);
            _tracker.Pos32 += 4;
        }
        else
        {
            Debug.Assert(increment <= 0);
        }

        WriteMultiBytes(buffer, increment, ref _tracker.Pos16);

        _tracker.Realign16_8();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write32BitAligned(scoped ReadOnlySpan<byte> streamRentBuffer)
    {
        var increment = GetIncrementLength(_tracker.Pos32);

        WriteMultiBytes(streamRentBuffer, increment, ref _tracker.Pos32);

        _tracker.Align32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void PadStream()
    {
        var remainder = (int)(Stream.Length & 3);
        switch (remainder)
        {
            case 1: Stream.WriteByte(0); Stream.WriteByte(0); Stream.WriteByte(0); break;
            case 2: Stream.WriteByte(0); Stream.WriteByte(0); break;
            case 3: Stream.WriteByte(0); break;
        }
    }

    public void Dispose()
    {
        if (_disposeStream)
            Stream.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if NET8_0_OR_GREATER
    private void WriteStringCore(ReadOnlySpan<char> value, int increment, int byteCount)
#else
    private void WriteStringCore(string value, int increment, int byteCount)
#endif
    {
        var sizeHint = increment > 0 ? byteCount + increment : byteCount;
        var span = Stream.GetSpan(sizeHint);

        if (increment > 0)
        {
            ClearSpan(span, increment);
            span = span.Slice(increment);
        }

#if NET8_0_OR_GREATER
        int bytesWritten = _encoding.GetBytes(value, span);
        span[bytesWritten] = 0; // 添加结尾的0字节
#else
        int bytesWritten = byteCount - 1;
        using (var rentedArray = new RentedArray<byte>(ArrayPool<byte>.Shared, bytesWritten))
        {
            int bytesEncoded = _encoding.GetBytes(value, 0, value.Length, rentedArray.Array, 0);
            rentedArray.Array.AsSpan(0, bytesEncoded).CopyTo(span);
        }

        span[bytesWritten] = 0; // 添加结尾的0字节
#endif
        Stream.Advance(sizeHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteBinaryCore(ReadOnlySpan<char> value, int increment, int length)
    {
        var sizeHint = increment > 0 ? length + increment : length;
        var span = Stream.GetSpan(sizeHint);

        if (increment > 0)
        {
            ClearSpan(span, increment);
            span = span.Slice(increment);
        }

#if NET9_0_OR_GREATER
        Convert.FromHexString(value, span.Slice(0, length), out var charsConsumed, out var bytesWritten);
#else
        HexConverter.TryDecodeFromUtf16(value, span.Slice(0, length));
#endif

        Stream.Advance(sizeHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteSingleByte(byte value, int increment, ref int pointer)
    {
        if (increment >= 0)
        {
            var sizeHint = increment + 1;
            var span = Stream.GetSpan(sizeHint);
            if (increment > 0)
            {
                ClearSpan(span, increment);
                span[increment] = value;
            }
            else
            {
                span[0] = value;
            }

            Stream.Advance(sizeHint);
        }
        else
        {
            var streamPosition = Stream.Position;
            Stream.Position = pointer;
            Stream.WriteByte(value);
            Stream.Position = streamPosition;
        }

        pointer++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteMultiBytes(scoped ReadOnlySpan<byte> buffer, int increment, ref int pointer)
    {
        if (increment >= 0)
        {
            var sizeHint = increment + buffer.Length;
            var span = Stream.GetSpan(sizeHint);
            if (increment > 0)
            {
                ClearSpan(span, increment);
                buffer.CopyTo(span.Slice(increment));
            }
            else
            {
                buffer.CopyTo(span);
            }

            Stream.Advance(sizeHint);
        }
        else
        {
            var streamPosition = Stream.Position;
            Stream.Position = pointer;
            Stream.Write(buffer);

            // fix the problem if the buffer length is greater than list count
            // but looks safe for kbin algorithm
            //if (offset <= Stream.Length)
            Stream.Position = streamPosition;
        }

        pointer += buffer.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private int GetIncrementLength(int pointer)
    {
        return pointer - (int)Stream.Length;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static void ClearSpan(Span<byte> span, int increment)
    {
        if (increment > 0)
        {
            span.Slice(0, increment).Clear();
        }
    }

    internal byte[] DebugGetArray()
    {
        PadStream();
        return Stream.ToArray();
    }
}