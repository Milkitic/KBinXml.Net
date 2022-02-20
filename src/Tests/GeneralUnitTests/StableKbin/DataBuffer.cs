using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace StableKbin
{
    internal sealed class DataBuffer : BigEndianBinaryBuffer
    {
        private Encoding _encoding;
        private int _pos32, _pos16, _pos8;

        public DataBuffer(byte[] buffer, Encoding encoding) : base(buffer)
        {
            _encoding = encoding;
        }

        public DataBuffer(Encoding encoding)
        {
            _encoding = encoding;
        }

        private void Realign16_8()
        {
            if (_pos8 % 4 == 0)
                _pos8 = _pos32;

            if (_pos16 % 4 == 0)
                _pos16 = _pos32;
        }

        private Span<byte> ReadBytes(ref int offset, int count)
        {
            if (Stream.Position == offset)
            {
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
                var span = count <= 128
                    ? stackalloc byte[count]
                    : new byte[count];
                Stream.Read(span);
                return span.ToArray();
#elif NETSTANDARD2_0
                var span = new byte[count];
                Stream.Read(span, 0, count);
                return span;
#endif
            }
            else
            {
                var pos = Stream.Position;
                Stream.Position = offset;

#if NETSTANDARD2_1 || NET5_0_OR_GREATER
                var span = count <= 128
                    ? stackalloc byte[count]
                    : new byte[count];
                Stream.Read(span);
#elif NETSTANDARD2_0
                var span = new byte[count];
                Stream.Read(span, 0, count);
#endif
                Stream.Position = pos;

#if NETSTANDARD2_1 || NET5_0_OR_GREATER
                return span.ToArray();
#elif NETSTANDARD2_0
                return span;
#endif
            }
        }

        public Span<byte> Read32BitAligned(int count)
        {
            var result = ReadBytes(ref _pos32, count);
            while (count % 4 != 0)
                count++;
            _pos32 += count;

            Realign16_8();

            return result;
        }

        public Span<byte> Read16BitAligned()
        {
            if (_pos16 % 4 == 0)
                _pos32 += 4;

            var result = ReadBytes(ref _pos16, 2);
            _pos16 += 2;
            Realign16_8();

            return result;
        }

        public Span<byte> Read8BitAligned()
        {
            if (_pos8 % 4 == 0)
                _pos32 += 4;

            var result = ReadBytes(ref _pos8, 1);
            _pos8++;
            Realign16_8();

            return result;
        }

        public void Write32BitAligned(Span<byte> buffer)
        {
            while (_pos32 > Stream.Length)
                Stream.WriteByte(0);

            SetRange(buffer, ref _pos32);
            while (_pos32 % 4 != 0)
                _pos32++;

            Realign16_8();
        }

        public void Write16BitAligned(Span<byte> buffer)
        {
            while (_pos16 > Stream.Length)
                Stream.WriteByte(0);

            if (_pos16 % 4 == 0)
                _pos32 += 4;

            SetRange(buffer, ref _pos16);
            Realign16_8();
        }

        public void Write8BitAligned(byte value)
        {
            while (_pos8 > Stream.Length)
                Stream.WriteByte(0);

            if (_pos8 % 4 == 0)
                _pos32 += 4;

            SetRange(new[] { value }, ref _pos8);
            Realign16_8();
        }

        public override Span<byte> ReadBytes(int count)
        {
            switch (count)
            {
                case 1:
                    return Read8BitAligned();

                case 2:
                    return Read16BitAligned();

                default:
                    return Read32BitAligned(count);
            }
        }

        public override void WriteBytes(Span<byte> buffer)
        {
            switch (buffer.Length)
            {
                case 1:
                    Write8BitAligned(buffer[0]);
                    break;

                case 2:
                    Write16BitAligned(buffer);
                    break;

                default:
                    Write32BitAligned(buffer);
                    break;
            }
        }

        public void WriteString(string value)
        {
            var bytes = _encoding.GetBytes(value);

            var length = bytes.Length + 1;
            var array = length <= 128
                ? stackalloc byte[length]
                : new byte[length];

            bytes.CopyTo(array);

            WriteU32((uint)array.Length);
            Write32BitAligned(array);
        }

        public string ReadString(int count)
        {
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
            return _encoding.GetString(Read32BitAligned(count)).TrimEnd('\0');
#elif NETSTANDARD2_0
            return _encoding.GetString(Read32BitAligned(count).ToArray()).TrimEnd('\0');
#endif
        }

        private static byte[] ConvertHexString(string hexString) => Enumerable.Range(0, hexString.Length)
            .Where(x => x % 2 == 0)
            .Select(x => byte.Parse(hexString.Substring(x, 2), NumberStyles.HexNumber))
            .ToArray();

        public void WriteBinary(string value)
        {
            WriteU32((uint)value.Length / 2);
            Write32BitAligned(ConvertHexString(value));
        }

        public string ReadBinary(int count)
        {
            return BitConverter.ToString(Read32BitAligned(count).ToArray()).Replace("-", "").ToLower();
        }

        private void SetRange(Span<byte> buffer, ref int offset)
        {
            if (offset == Stream.Length)
            {
#if NETSTANDARD2_0
                Stream.Write(buffer.ToArray(), 0, buffer.Length);
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
                Stream.Write(buffer);
#endif
                offset += buffer.Length;
            }
            else
            {
                var pos = Stream.Position;
                Stream.Position = offset;

#if NETSTANDARD2_0
                Stream.Write(buffer.ToArray(), 0, buffer.Length);
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
                Stream.Write(buffer);
#endif

                offset += buffer.Length;

                Stream.Position = pos;
            }
        }
    }
}
