using System;
using System.Buffers;
using System.Text;
using KbinXml.Net.Internal;

namespace KbinXml.Net.Readers;

internal class DataReader : BeBinaryReader
{
    private readonly Encoding _encoding;
    private int _pos32;
    private int _pos16;
    private int _pos8;

    public DataReader(Memory<byte> buffer, Encoding encoding) : base(buffer)
    {
        _encoding = encoding;
    }

    public Memory<byte> Read32BitAligned(int count)
    {
        var result = ReadBytes(_pos32, count);
        while (count % 4 != 0)
            count++;
        _pos32 += count;

        Realign16_8();

        return result;
    }

    public Memory<byte> Read16BitAligned()
    {
        if (_pos16 % 4 == 0)
            _pos32 += 4;

        var result = ReadBytes(_pos16, 2);
        _pos16 += 2;
        Realign16_8();

        return result;
    }

    public Memory<byte> Read8BitAligned()
    {
        if (_pos8 % 4 == 0)
            _pos32 += 4;

        var result = ReadBytes(_pos8, 1);
        _pos8++;
        Realign16_8();

        return result;
    }

    public override Memory<byte> ReadBytes(int count)
    {
        return count switch
        {
            1 => Read8BitAligned(),
            2 => Read16BitAligned(),
            _ => Read32BitAligned(count)
        };
    }

    public string ReadString(int count)
    {
        var memory = Read32BitAligned(count);
        var span = memory.Span.Slice(0, memory.Length - 1);
        if (span.Length == 0)
            return string.Empty;

#if NETCOREAPP3_1_OR_GREATER
        return _encoding.GetString(span);
#else
        unsafe
        {
            fixed (byte* p = span)
                return _encoding.GetString(p, span.Length);
        }
#endif
    }

    public string ReadBinary(int count)
    {
        var bin = Read32BitAligned(count);
        if (bin.Length == 0)
            return string.Empty;
#if NETCOREAPP3_1_OR_GREATER
        var str = string.Create(bin.Length * 2, bin, static (dst, state) =>
        {
            var src = state.Span;

            int i = 0;
            int j = 0;

            while (i < src.Length)
            {
                var b = src[i++];
                dst[j++] = ToCharLower(b >> 4);
                dst[j++] = ToCharLower(b);
            }
        });
        return str;
#else
        var src = bin.Span;
        var dstLen = bin.Length * 2;

        char[]? arr = null;
        Span<char> dst = dstLen <= Constants.MaxStackLength
            ? stackalloc char[dstLen]
            : arr = ArrayPool<char>.Shared.Rent(dstLen);
        if (arr != null) dst = dst.Slice(0, dstLen);
        try
        {
            int i = 0;
            int j = 0;

            while (i < bin.Length)
            {
                var b = src[i++];
                dst[j++] = ToCharLower(b >> 4);
                dst[j++] = ToCharLower(b);
            }

            unsafe
            {
                fixed (char* p = dst)
                    return new string(p, 0, dstLen);
            }
        }
        finally
        {
            if (arr != null) ArrayPool<char>.Shared.Return(arr);
        }
#endif
    }

    private Memory<byte> ReadBytes(int offset, int count)
    {
        var slice = Buffer.Slice(offset, count);
        return slice;
    }

    private void Realign16_8()
    {
        if (_pos8 % 4 == 0)
            _pos8 = _pos32;

        if (_pos16 % 4 == 0)
            _pos16 = _pos32;
    }

    private static char ToCharLower(int value)
    {
        value &= 0xF;
        value += '0';

        if (value > '9') value += ('a' - ('9' + 1));
        return (char)value;
    }
}