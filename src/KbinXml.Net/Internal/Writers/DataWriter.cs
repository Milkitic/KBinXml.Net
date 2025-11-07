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
    private readonly Encoding _encoding;
    private readonly bool _disposeStream;

    private WriteContextManager _writeContextManager;

    public DataWriter(Encoding encoding, int capacity = 0)
    {
        _encoding = encoding;
        Stream = KbinConverter.RecyclableMemoryStreamManager.GetStream("wd", capacity);
        _disposeStream = true;
        _writeContextManager = new WriteContextManager(Stream);
    }

    public DataWriter(Encoding encoding, RecyclableMemoryStream stream)
    {
        _encoding = encoding;
        Stream = stream;
        _writeContextManager = new WriteContextManager(Stream);
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

        var buffer = _writeContextManager.BeginWrite32Trust(byteCount);
#if NET8_0_OR_GREATER
        int bytesWritten = _encoding.GetBytes(value, buffer);
        buffer[bytesWritten] = 0; // 添加结尾的0字节
#else
        int bytesWritten = byteCount - 1;
        using (var rentedArray = new RentedArray<byte>(ArrayPool<byte>.Shared, bytesWritten))
        {
            int bytesEncoded = _encoding.GetBytes(value, 0, value.Length, rentedArray.Array, 0);
            rentedArray.Array.AsSpan(0, bytesEncoded).CopyTo(buffer);
        }

        buffer[bytesWritten] = 0; // 添加结尾的0字节
#endif
        _writeContextManager.EndWrite32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBinary(ReadOnlySpan<char> value)
    {
        // 计算二进制数据的长度（每两个字符表示一个字节）
        int length = value.Length >> 1;

        // 先写入长度
        WriteU32((uint)length);

        var buffer = _writeContextManager.BeginWrite32Trust(length);
#if NET9_0_OR_GREATER
        Convert.FromHexString(value, buffer, out var charsConsumed, out var bytesWritten);
#else
        HexConverter.TryDecodeFromUtf16(value, buffer.Slice(0, length));
#endif
        _writeContextManager.EndWrite32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write8BitAligned(byte value)
    {
        _writeContextManager.Write(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write16BitAligned(scoped ReadOnlySpan<byte> span)
    {
        _writeContextManager.Write16(span);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write32BitAligned(scoped ReadOnlySpan<byte> span)
    {
        var buffer = _writeContextManager.BeginWrite32(span.Length);
        span.CopyTo(buffer);
        _writeContextManager.EndWrite32();
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

    internal byte[] DebugGetArray()
    {
        PadStream();
        return Stream.ToArray();
    }
}