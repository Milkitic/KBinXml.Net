using System;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Readers;

internal class BeBinaryReader
{
    protected readonly Memory<byte> Buffer;
    private int _position = 0;

    public BeBinaryReader(Memory<byte> buffer)
    {
        Buffer = buffer;
    }

    public virtual Memory<byte> ReadBytes(int count)
    {
        var slice = Buffer.Slice(_position, count);
        _position += count;
        return slice;
    }

    public virtual sbyte ReadS8() => (sbyte)ReadBytes(sizeof(byte)).Span[0];
    public virtual short ReadS16() => BitConverterHelper.ToBeInt16(ReadBytes(sizeof(short)).Span);
    public virtual int ReadS32() => BitConverterHelper.ToBeInt32(ReadBytes(sizeof(int)).Span);
    public virtual long ReadS64() => BitConverterHelper.ToBeInt64(ReadBytes(sizeof(short)).Span);
    public virtual byte ReadU8() => ReadBytes(sizeof(byte)).Span[0];
    public virtual ushort ReadU16() => BitConverterHelper.ToBeUInt16(ReadBytes(sizeof(short)).Span);
    public virtual uint ReadU32() => BitConverterHelper.ToBeUInt32(ReadBytes(sizeof(int)).Span);
    public virtual ulong ReadU64() => BitConverterHelper.ToBeUInt64(ReadBytes(sizeof(long)).Span);

}