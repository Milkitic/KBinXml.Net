using System;
using System.Text;

namespace StableKbin
{
    internal sealed class NodeBuffer : BigEndianBinaryBuffer
    {
        private readonly Encoding _encoding;
        private readonly bool _compressed;

        internal NodeBuffer(bool compressed, Encoding encoding)
        {
            _compressed = compressed;
            _encoding = encoding;
        }

        internal NodeBuffer(byte[] buffer, bool compressed, Encoding encoding)
            : base(buffer)
        {
            _compressed = compressed;
            _encoding = encoding;
        }

        internal void WriteString(string value)
        {
            if (_compressed)
            {
                WriteU8((byte)value.Length);
                WriteBytes(Sixbit.Encode(value));
            }
            else
            {
                WriteU8((byte)((value.Length - 1) | (1 << 6)));
                WriteBytes(_encoding.GetBytes(value));
            }
        }

        internal string ReadString()
        {
            int length = ReadU8();

            if (_compressed)
                return Sixbit.Decode(ReadBytes((int)Math.Ceiling(length * 6 / 8.0)), length);

#if NETSTANDARD2_1 || NET5_0_OR_GREATER
            return _encoding.GetString(ReadBytes((length & 0xBF) + 1));
#elif NETSTANDARD2_0
            return _encoding.GetString(ReadBytes((length & 0xBF) + 1).ToArray());
#endif
        }
    }
}
