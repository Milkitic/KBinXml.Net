using System;
using System.Buffers;
using System.Runtime.CompilerServices;
using System.Text;
using KbinXml.Net.Utils;
using Microsoft.IO;

namespace KbinXml.Net.Internal.Writers;

internal ref partial struct DataWriter : IKBinWriter, IDisposable
{
    internal readonly RecyclableMemoryStream Stream;
    private readonly bool _zeroFillGap;
    private readonly Encoding _encoding;
    private readonly bool _disposeStream;

    private WriteContextManager _writeContextManager;

    public DataWriter(Encoding encoding, int capacity = 0) : this(true, encoding, capacity)
    {
    }

    public DataWriter(Encoding encoding, RecyclableMemoryStream stream) : this(true, encoding, stream)
    {
    }

    public DataWriter(bool zeroFillGap, Encoding encoding, int capacity = 0)
    {
        _zeroFillGap = zeroFillGap;
        _encoding = encoding;
        Stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("wd", capacity);
        _disposeStream = true;
        _writeContextManager = new WriteContextManager(zeroFillGap, Stream);
    }

    public DataWriter(bool zeroFillGap, Encoding encoding, RecyclableMemoryStream stream)
    {
        _zeroFillGap = zeroFillGap;
        _encoding = encoding;
        Stream = stream;
        _writeContextManager = new WriteContextManager(zeroFillGap, Stream);
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

#if !NETSTANDARD2_0 && !NETFRAMEWORK
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

        int alignedLength;
        int padding;
        if (_zeroFillGap)
        {
            alignedLength = (byteCount + 3) & ~3;
            padding = alignedLength - byteCount;
        }
        else
        {
            alignedLength = byteCount;
            padding = 0;
        }

        var buffer = _writeContextManager.BeginWrite32Sequential(alignedLength);
#if !NETSTANDARD2_0 && !NETFRAMEWORK
        int bytesWritten = _encoding.GetBytes(value, buffer);
#else
        int bytesWritten = byteCount - 1;
        using (var rentedArray = new RentedArray<byte>(ArrayPool<byte>.Shared, bytesWritten))
        {
            int bytesEncoded = _encoding.GetBytes(value, 0, value.Length, rentedArray.Array, 0);
            rentedArray.Array.AsSpan(0, bytesEncoded).CopyTo(buffer);
        }
#endif
        buffer[bytesWritten] = 0; // 添加结尾的0字节
        if (_zeroFillGap && padding != 0)
        {
            buffer.Slice(byteCount, padding).Clear();
        }

        _writeContextManager.EndWrite32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBinary(ReadOnlySpan<char> value)
    {
        // 计算二进制数据的长度（每两个字符表示一个字节）
        int length = value.Length >> 1;

        // 先写入长度
        WriteU32((uint)length);

        int alignedLength;
        int padding;
        if (_zeroFillGap)
        {
            alignedLength = (length + 3) & ~3;
            padding = alignedLength - length;
        }
        else
        {
            alignedLength = length;
            padding = 0;
        }

        var buffer = _writeContextManager.BeginWrite32Sequential(alignedLength);
#if NET9_0_OR_GREATER
        Convert.FromHexString(value, buffer, out _, out _);
#else
        HexConverter.TryDecodeFromUtf16(value, buffer.Slice(0, length));
#endif
        if (_zeroFillGap && padding != 0)
        {
            buffer.Slice(length, padding).Clear();
        }

        _writeContextManager.EndWrite32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8BitAligned(byte value)
    {
        _writeContextManager.Write8(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write16BitAligned(scoped ReadOnlySpan<byte> span)
    {
        _writeContextManager.Write16(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write32BitAligned(scoped ReadOnlySpan<byte> span)
    {
        _writeContextManager.Write32(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void FinalizeData()
    {
        _writeContextManager.FinalizeData();
    }

    public void Dispose()
    {
        if (_disposeStream)
            Stream.Dispose();
    }

    internal byte[] DebugGetArray()
    {
        FinalizeData();
        return Stream.ToArray();
    }
}