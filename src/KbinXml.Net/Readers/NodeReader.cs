using System;
using System.Text;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Readers;

internal class NodeReader : BeBinaryReader
{
    private readonly bool _compressed;
    private readonly Encoding _encoding;

    public NodeReader(Memory<byte> buffer, int offset, bool compressed, Encoding encoding)
        : base(buffer, offset)
    {
        _compressed = compressed;
        _encoding = encoding;
    }

    public string ReadString(out int position)
    {
        int length = ReadU8(out position, out _);

        if (_compressed)
        {
            var memory = ReadBytes((int)Math.Ceiling(length * 6 / 8.0), out position, out _);
            return SixbitHelper.Decode(memory.Span, length);
        }

        var mem = ReadBytes((length & 0xBF) + 1, out position, out _);
#if NETSTANDARD2_1 || NETCOREAPP3_1_OR_GREATER
        return _encoding.GetString(mem.Span);
#elif NETSTANDARD2_0 || NET46_OR_GREATER
        unsafe
        {
            fixed (byte* p = mem.Span)
                return _encoding.GetString(p, mem.Length);
        }
#else
        return _encoding.GetString(mem.ToArray());
#endif
    }
}