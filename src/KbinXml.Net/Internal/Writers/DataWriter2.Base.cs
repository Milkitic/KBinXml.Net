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
        Write16BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU16(ushort value)
    {
        Write16BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS32(int value)
    {
        Write32BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU32(uint value)
    {
        Write32BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteS64(long value)
    {
        Write32BitAlignedInternal(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteU64(ulong value)
    {
        Write32BitAlignedInternal(value);
    }

    private void Write16BitAlignedInternal<T>(T value) where T : unmanaged
    {
        const int size = 2; // sizeof(short) or sizeof(ushort)
        var buffer = _writeContextManager.BeginWrite16(size);
        BitConverterHelper.WriteBeBytesT(buffer, value);
        _writeContextManager.EndWrite16();
    }

    private void Write32BitAlignedInternal<T>(T value) where T : unmanaged
    {
        int size = Unsafe.SizeOf<T>();
        var buffer = _writeContextManager.BeginWrite32(size);
        BitConverterHelper.WriteBeBytesT(buffer, value);
        _writeContextManager.EndWrite32();
    }
}