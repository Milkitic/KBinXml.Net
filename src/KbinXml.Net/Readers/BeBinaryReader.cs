using System;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Readers;

internal class BeBinaryReader
{
    protected readonly Memory<byte> Buffer;
    protected readonly int Offset;

    // ReSharper disable once InconsistentNaming
    protected int _position;

    public BeBinaryReader(Memory<byte> buffer, int offset = 0)
    {
        Buffer = buffer;
        Offset = offset;
    }

    public int Position => _position + Offset;

    public virtual Memory<byte> ReadBytes(int count, out int position, out string? flag)
    {
        position = _position + Offset;
        var slice = Buffer.Slice(_position, count);
        _position += count;
        flag = null;
        return slice;
    }

    public virtual sbyte ReadS8(out int position, out string? flag) =>
        (sbyte)ReadBytes(sizeof(byte), out position, out flag).Span[0];

    public virtual short ReadS16(out int position, out string? flag) =>
        BitConverterHelper.ToBeInt16(ReadBytes(sizeof(short), out position, out flag).Span);

    public virtual int ReadS32(out int position, out string? flag) =>
        BitConverterHelper.ToBeInt32(ReadBytes(sizeof(int), out position, out flag).Span);

    public virtual long ReadS64(out int position, out string? flag) =>
        BitConverterHelper.ToBeInt64(ReadBytes(sizeof(short), out position, out flag).Span);

    public virtual byte ReadU8(out int position, out string? flag) =>
        ReadBytes(sizeof(byte), out position, out flag).Span[0];

    public virtual ushort ReadU16(out int position, out string? flag) =>
        BitConverterHelper.ToBeUInt16(ReadBytes(sizeof(short), out position, out flag).Span);

    public virtual uint ReadU32(out int position, out string? flag) =>
        BitConverterHelper.ToBeUInt32(ReadBytes(sizeof(int), out position, out flag).Span);

    public virtual ulong ReadU64(out int position, out string? flag) =>
        BitConverterHelper.ToBeUInt64(ReadBytes(sizeof(long), out position, out flag).Span);
}