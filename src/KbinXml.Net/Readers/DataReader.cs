﻿using System;
using System.Runtime.CompilerServices;
using System.Text;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Readers;

internal class DataReader : BeBinaryReader
{
    private readonly Encoding _encoding;
    private int _pos16;
    private int _pos8;

    public DataReader(Memory<byte> buffer, int baseOffset, Encoding encoding) : base(buffer, baseOffset)
    {
        _encoding = encoding;
    }

    //public int Position32 => _position + BaseOffset;
    //public int Position16 => _pos16 + BaseOffset;
    //public int Position8 => _pos8 + BaseOffset;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte> Read32BitAligned(int count, out int position, out string flag)
    {
#if DEBUG
        position = _position + BaseOffset;
#else
        position = _position;
#endif
        flag = "p32";
        var result = ReadBytes(_position, count);
        var left = count & 3;
        if (left != 0)
        {
            count += (4 - left);
        }

        _position += count;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte> Read16BitAligned(out int position, out string flag)
    {
        // Realign before read.
        // If need to, align pos16 to next 4-bytes chunk, and move the generic position to next chunk
        if ((_pos16 & 3) == 0)
        {
#if DEBUG
            if (_pos16 != _position)
            {
                Console.WriteLine($"---> p16 from {_pos16 + BaseOffset:X8} to {_position + BaseOffset:X8}");
            }
#endif
            _pos16 = _position;

#if DEBUG
            Console.WriteLine($"---> p32 from {_position + BaseOffset:X8} to {_position + BaseOffset + 4:X8}");
#endif
            _position += 4;
        }

#if DEBUG
        position = _pos16 + BaseOffset;
#else
        position = _pos16;
#endif

        flag = "p16";
        var result = ReadBytes(_pos16, 2);
        _pos16 += 2;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Memory<byte> Read8BitAligned(out int position, out string flag)
    {
        // Realign before read.
        // If need to, align pos8 to next 4-bytes chunk, and move the generic position to next chunk
        if ((_pos8 & 3) == 0)
        {
#if DEBUG
            if (_pos8 != _position)
            {
                Console.WriteLine($"---> p8 from {_pos8 + BaseOffset:X8} to {_position + BaseOffset:X8}");
            }
#endif
            _pos8 = _position;

#if DEBUG
            Console.WriteLine($"---> p32 from {_position + BaseOffset:X8} to {_position + BaseOffset + 4:X8}");
#endif
            _position += 4;
        }

#if DEBUG
        position = _pos8 + BaseOffset;
#else
        position = _pos8;
#endif
        flag = "p8";

        var result = ReadBytes(_pos8, 1);
        _pos8++;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override Memory<byte> ReadBytes(int count, out int position, out string flag)
    {
        return count switch
        {
            1 => Read8BitAligned(out position, out flag),
            2 => Read16BitAligned(out position, out flag),
            _ => Read32BitAligned(count, out position, out flag)
        };
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadString(int count, out int position, out string flag)
    {
        var memory = Read32BitAligned(count, out position, out flag);
        var span = memory.Span.Slice(0, memory.Length - 1);
        if (span.Length == 0)
            return string.Empty;

#if NETCOREAPP3_1_OR_GREATER
        return _encoding.GetString(span);
#elif NETSTANDARD2_0 || NET46_OR_GREATER
        unsafe
        {
            fixed (byte* p = span)
                return _encoding.GetString(p, span.Length);
        }
#else
        return _encoding.GetString(span.ToArray());
#endif
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string ReadBinary(int count, out int position, out string flag)
    {
        var bin = Read32BitAligned(count, out position, out flag);
        if (bin.Length == 0)
            return string.Empty;
        return ConvertHelper.ToHexString(bin.Span);
    }

    [InlineMethod.Inline]
    private Memory<byte> ReadBytes(int offset, int count)
    {
        int actualCount;
        if (count + offset > Buffer.Length)
            actualCount = Buffer.Length - offset;
        else
            actualCount = count;
        var slice = Buffer.Slice(offset, actualCount);
        return slice;
    }
}