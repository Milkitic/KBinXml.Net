using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using KbinXml.Net.Internal;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Writers;

public class DataWriter : BeBinaryWriter
{
    private int _pos32;
    private int _pos16;
    private int _pos8;
    private readonly Encoding _encoding;
    private readonly int _shiftVal;

    public DataWriter(Encoding encoding)
    {
        _encoding = encoding;
        _shiftVal = EncodingDictionary.ReverseEncodingMap[encoding] switch
        {
            0x00 => 1,
            0x20 => 1,
            0x40 => 1,
            0x60 => 2,
            0x80 => 2,
            0xA0 => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(encoding), encoding, null)
        };
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteBytes(ReadOnlySpan<byte> buffer)
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

    public void WriteString(string value)
    {
#if NETCOREAPP3_1_OR_GREATER
        var maxLen = (value.Length << _shiftVal) + 1;
        bool useStack = maxLen <= Constants.MaxStackLength;
        if (!useStack)
        {
            maxLen = _encoding.GetByteCount(value) + 1;
            useStack = maxLen <= Constants.MaxStackLength;
            if (useStack)
            {
                Span<byte> buffer = stackalloc byte[maxLen];
                _encoding.GetBytes(value.AsSpan(), buffer);
                WriteU32((uint)maxLen);
                Write32BitAligned(buffer);
                return;
            }
        }

        if (useStack)
        {
            Span<byte> buffer = stackalloc byte[maxLen];
            var count = _encoding.GetBytes(value.AsSpan(), buffer);
            var fullCount = count + 1;
            var fullBytes = buffer.Slice(0, fullCount);
            WriteU32((uint)fullCount);
            Write32BitAligned(fullBytes);
            return;
        }

        var bytes = _encoding.GetBytes(value);
        var length = bytes.Length + 1;
        var arr = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            var span = arr.AsSpan(0, length);
            bytes.CopyTo(span);
            span[bytes.Length] = 0;

            WriteU32((uint)length);
            Write32BitAligned(span);
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(arr);
        }
#else
            var bytes = _encoding.GetBytes(value);

            var length = bytes.Length + 1;
            byte[]? arr = null;
            Span<byte> span = length <= Constants.MaxStackLength
                ? stackalloc byte[length]
                : arr = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                if (arr != null) span = span.Slice(0, length);
                bytes.CopyTo(span);

                WriteU32((uint)length);
                Write32BitAligned(span);
            }
            finally
            {
                if (arr != null) ArrayPool<byte>.Shared.Return(arr);
            }
#endif
    }

    public void WriteBinary(string value)
    {
        var length = value.Length >> 1;
        WriteU32((uint)length);
        byte[]? arr = null;
        Span<byte> span = length <= Constants.MaxStackLength
            ? stackalloc byte[length]
            : arr = ArrayPool<byte>.Shared.Rent(length);
        if (arr != null) span = span.Slice(0, length);
        try
        {
            HexConverter.TryDecodeFromUtf16(value.AsSpan(), span);
            Write32BitAligned(span);
        }
        finally
        {
            if (arr != null) ArrayPool<byte>.Shared.Return(arr);
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write32BitAligned(ReadOnlySpan<byte> buffer)
    {
        Pad(_pos32);

        WriteBytes(buffer, ref _pos32);
        while ((_pos32 & 3) != 0)
        {
            _pos32++;
        }

        Realign16_8();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write16BitAligned(ReadOnlySpan<byte> buffer)
    {
        Pad(_pos16);

        if ((_pos16 & 3) == 0)
        {
            _pos32 += 4;
        }

        WriteBytes(buffer, ref _pos16);
        Realign16_8();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8BitAligned(byte value)
    {
        Pad(_pos8);

        if ((_pos8 & 3) == 0)
        {
            _pos32 += 4;
        }

        WriteBytes(stackalloc[] { value }, ref _pos8);
        Realign16_8();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void WriteBytes(ReadOnlySpan<byte> buffer, ref int offset)
    {
        if (offset == Stream.Length)
        {
            Stream.WriteSpan(buffer);
            offset += buffer.Length;
        }
        else
        {
            var pos = Stream.Position;
            Stream.Position = offset;

            Stream.WriteSpan(buffer);

            offset += buffer.Length;

            // fix the problem if the buffer length is greater than list count
            // but looks safe for kbin algorithm
            //if (offset <= Stream.Length)
            Stream.Position = pos;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Realign16_8()
    {
        if ((_pos8 & 3) == 0)
        {
            _pos8 = _pos32;
        }

        if ((_pos16 & 3) == 0)
        {
            _pos16 = _pos32;
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void Pad(int target)
    {
        int left = (int)(target - Stream.Length);
        if (left <= 0) return;
#if NETCOREAPP3_1_OR_GREATER
        if (left == 1) Stream.WriteByte(0);
        else
        {
            // looks safe for kbin algorithm
            Stream.Write(stackalloc byte[left]);
            //byte[]? arr = null;
            //Span<byte> span = left <= Constants.MaxStackLength
            //    ? stackalloc byte[left]
            //    : arr = ArrayPool<byte>.Shared.Rent(left);
            //if (arr != null) span = span.Slice(0, left);
            //try
            //{
            //    Stream.Write(span);
            //}
            //finally
            //{
            //    if (arr != null) ArrayPool<byte>.Shared.Return(arr);
            //}
        }
#else
            for (int i = 0; i < left; i++)
                Stream.WriteByte(0);
#endif
    }
}