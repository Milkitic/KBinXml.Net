using System;
using System.IO;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Writers;

internal class BeBinaryWriter : IDisposable
{
    protected internal readonly MemoryStream Stream;

    public BeBinaryWriter()
    {
        Stream = new MemoryStream(0);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteBytes(ReadOnlySpan<byte> buffer)
    {
        Stream.WriteSpan(buffer);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteS8(sbyte value)
    {
        WriteBytes(stackalloc[] { (byte)value });
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteS16(short value)
    {
        Span<byte> span = stackalloc byte[sizeof(short)];
        var builder = new ValueListBuilder<byte>(span);
        BitConverterHelper.WriteBeBytes(ref builder, value);
        WriteBytes(builder.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteS32(int value)
    {
        Span<byte> span = stackalloc byte[sizeof(int)];
        var builder = new ValueListBuilder<byte>(span);
        BitConverterHelper.WriteBeBytes(ref builder, value);
        WriteBytes(builder.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteS64(long value)
    {
        Span<byte> span = stackalloc byte[sizeof(long)];
        var builder = new ValueListBuilder<byte>(span);
        BitConverterHelper.WriteBeBytes(ref builder, value);
        WriteBytes(builder.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteU8(byte value)
    {
        Stream.WriteByte(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteU16(ushort value)
    {
        Span<byte> span = stackalloc byte[sizeof(ushort)];
        var builder = new ValueListBuilder<byte>(span);
        BitConverterHelper.WriteBeBytes(ref builder, value);
        WriteBytes(builder.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteU32(uint value)
    {
        Span<byte> span = stackalloc byte[sizeof(uint)];
        var builder = new ValueListBuilder<byte>(span);
        BitConverterHelper.WriteBeBytes(ref builder, value);
        WriteBytes(builder.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public virtual void WriteU64(ulong value)
    {
        Span<byte> span = stackalloc byte[sizeof(ulong)];
        var builder = new ValueListBuilder<byte>(span);
        BitConverterHelper.WriteBeBytes(ref builder, value);
        WriteBytes(builder.AsSpan());
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Pad()
    {
        while ((Stream.Length & 3) != 0)
        {
            Stream.WriteByte(0);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public byte[] ToArray()
    {
        return Stream.ToArray();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Dispose()
    {
        Stream.Dispose();
    }
}