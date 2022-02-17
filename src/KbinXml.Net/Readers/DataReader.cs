using System;
using System.Buffers;
using System.Text;
using KbinXml.Net.Internal;
using KbinXml.Net.Utils;

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

    public string ReadBinary(int count)
    {
        var bin = Read32BitAligned(count);
        if (bin.Length == 0)
            return string.Empty;
        return ConvertHelper.ToHexString(bin.Span);
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
}