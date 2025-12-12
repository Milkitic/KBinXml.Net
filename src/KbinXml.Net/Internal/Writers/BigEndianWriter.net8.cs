#if NET8_0_OR_GREATER
using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Writers;

internal readonly ref partial struct BigEndianWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Write<T>(T value) where T : System.Numerics.IBinaryInteger<T>
    {
        int size = value.GetByteCount();
        if (_recyclableMemoryStream is { } stream)
        {
            BitConverterHelper.WriteBeBytes(stream.GetSpan(size), value);
            stream.Advance(size);
        }
        else
        {
            Span<byte> buffer = stackalloc byte[size];
            BitConverterHelper.WriteBeBytes(buffer, value);
            BaseStream.WriteSpan(buffer);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS16(short value) => Write(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU16(ushort value) => Write(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS32(int value) => Write(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU32(uint value) => Write(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS64(long value) => Write(value);
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU64(ulong value) => Write(value);
}
#endif