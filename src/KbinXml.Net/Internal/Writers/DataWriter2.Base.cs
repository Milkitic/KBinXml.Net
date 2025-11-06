using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Writers;

internal partial struct DataWriter2
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
        const int size = sizeof(short);
        var buffer = _writeContextManager.BeginWrite16(size);
        BitConverterHelper.WriteBeBytes(buffer, value);
        _writeContextManager.EndWrite16();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU16(ushort value)
    {
        const int size = sizeof(ushort);
        var buffer = _writeContextManager.BeginWrite16(size);
        BitConverterHelper.WriteBeBytes(buffer, value);
        _writeContextManager.EndWrite16();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS32(int value)
    {
        const int size = sizeof(int);
        var buffer = _writeContextManager.BeginWrite32(size);
        BitConverterHelper.WriteBeBytes(buffer, value);
        _writeContextManager.EndWrite32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU32(uint value)
    {
        const int size = sizeof(uint);
        var buffer = _writeContextManager.BeginWrite32(size);
        BitConverterHelper.WriteBeBytes(buffer, value);
        _writeContextManager.EndWrite32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS64(long value)
    {
        const int size = sizeof(long);
        var buffer = _writeContextManager.BeginWrite32(size);
        BitConverterHelper.WriteBeBytes(buffer, value);
        _writeContextManager.EndWrite32();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU64(ulong value)
    {
        const int size = sizeof(ulong);
        var buffer = _writeContextManager.BeginWrite32(size);
        BitConverterHelper.WriteBeBytes(buffer, value);
        _writeContextManager.EndWrite32();
    }

    //private void Write16BitAlignedInternal<T>(T value) where T : unmanaged
    //{
    //    const int size = 2; // sizeof(short) or sizeof(ushort)
    //    var buffer = _writeContextManager.BeginWrite16(size);
    //    BitConverterHelper.WriteBeBytesT(buffer, value);
    //    _writeContextManager.EndWrite16();
    //}

    //private void Write32BitAlignedInternal<T>(T value) where T : unmanaged
    //{
    //    int size = Unsafe.SizeOf<T>();
    //    var buffer = _writeContextManager.BeginWrite32(size);
    //    BitConverterHelper.WriteBeBytesT(buffer, value);
    //    _writeContextManager.EndWrite32();
    //}
}