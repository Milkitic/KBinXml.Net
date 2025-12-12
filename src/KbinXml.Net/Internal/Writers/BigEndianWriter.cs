using System;
using System.IO;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;
using Microsoft.IO;

namespace KbinXml.Net.Internal.Writers;

internal readonly ref partial struct BigEndianWriter : IKBinWriter, IDisposable
{
    public readonly Stream BaseStream;

    private readonly RecyclableMemoryStream? _recyclableMemoryStream;
    private readonly bool _leaveOpen;

    public BigEndianWriter(int capacity = 0)
    {
        BaseStream = _recyclableMemoryStream = KbinConverter.RecyclableMemoryStreamManager.GetStream("wbe", capacity);
    }

    public BigEndianWriter(Stream baseStream)
    {
        _leaveOpen = true;
        BaseStream = baseStream;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteByte(byte singleByte) => BaseStream.WriteByte(singleByte);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteBytes(scoped ReadOnlySpan<byte> buffer) => BaseStream.WriteSpan(buffer);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS8(sbyte value) => WriteByte((byte)value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU8(byte value) => WriteByte(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if !NETSTANDARD2_0
    [SkipLocalsInit]
#endif
    internal void Pad()
    {
        while ((BaseStream.Length & 3) != 0)
        {
            BaseStream.WriteByte(0);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete("This method has degraded performance and should be avoided.")]
    public byte[] ToArray() => BaseStream.ToArray();

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        if (!_leaveOpen)
            BaseStream.Dispose();
    }
}