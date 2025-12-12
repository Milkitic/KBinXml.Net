#if !NET8_0_OR_GREATER
using System;
using System.Runtime.CompilerServices;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Writers;

internal readonly ref partial struct BigEndianWriter
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
#if !NETSTANDARD2_0
    [SkipLocalsInit]
#endif
    public void WriteS16(short value)
    {
        const int size = sizeof(short);
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
#if !NETSTANDARD2_0
    [SkipLocalsInit]
#endif
    public void WriteU16(ushort value)
    {
        const int size = sizeof(ushort);
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
#if !NETSTANDARD2_0
    [SkipLocalsInit]
#endif
    public void WriteS32(int value)
    {
        const int size = sizeof(int);
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
#if !NETSTANDARD2_0
    [SkipLocalsInit]
#endif
    public void WriteU32(uint value)
    {
        const int size = sizeof(uint);
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
#if !NETSTANDARD2_0
    [SkipLocalsInit]
#endif
    public void WriteS64(long value)
    {
        const int size = sizeof(long);
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
#if !NETSTANDARD2_0
    [SkipLocalsInit]
#endif
    public void WriteU64(ulong value)
    {
        const int size = sizeof(ulong);
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
}
#endif
