using System;
using System.Linq;
using System.Text;

namespace KbinXml.Readers;

internal class DataReader : BeBinaryReader
{
    private readonly Encoding _encoding;
    private int _pos32, _pos16, _pos8;

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
#if NETCOREAPP3_1_OR_GREATER
        return _encoding.GetString(Read32BitAligned(count).Span).TrimEnd('\0');
#else
        return _encoding.GetString(Read32BitAligned(count).ToArray()).TrimEnd('\0');
#endif
    }

    public string ReadBinary(int count)
    {
        return BitConverter.ToString(Read32BitAligned(count).ToArray())
            .Replace("-", "")
            .ToLower();
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