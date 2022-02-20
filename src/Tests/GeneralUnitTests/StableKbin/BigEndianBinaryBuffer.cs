using System;
using System.IO;

namespace StableKbin
{
    public class BigEndianBinaryBuffer : IDisposable
    {
        protected readonly Stream Stream;

        public BigEndianBinaryBuffer(byte[] buffer)
        {
            Stream = new MemoryStream(buffer);
        }

        public BigEndianBinaryBuffer()
        {
            Stream = new MemoryStream();
        }

        public virtual Span<byte> ReadBytes(int count)
        {
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
            var span = count <= 128
                ? stackalloc byte[count]
                : new byte[count];
            Stream.Read(span);
            return span.ToArray();
#elif NETSTANDARD2_0
            var buffer = new byte[count];
            Stream.Read(buffer, 0, count);
            return buffer;
#endif
        }

        public virtual void WriteBytes(Span<byte> buffer)
        {
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
            Stream.Write(buffer);
#elif NETSTANDARD2_0
            Stream.Write(buffer.ToArray(), 0, buffer.Length);
#endif
        }

        public virtual void WriteS8(sbyte value) => WriteBytes(
#if NETSTANDARD2_0
            new[] { (byte)value }
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
            stackalloc[] { (byte)value }
#endif
        );

        public virtual void WriteS16(short value) => WriteBytes(BitConverterHelper.GetBigEndianBytes(
#if NETSTANDARD2_0
            new byte[sizeof(short)]
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
            stackalloc byte[sizeof(short)]
#endif
            , value));

        public virtual void WriteS32(int value) => WriteBytes(BitConverterHelper.GetBigEndianBytes(
#if NETSTANDARD2_0
            new byte[sizeof(int)]
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
            stackalloc byte[sizeof(int)]
#endif
            , value));

        public virtual void WriteS64(long value) => WriteBytes(BitConverterHelper.GetBigEndianBytes(
#if NETSTANDARD2_0
            new byte[sizeof(long)]
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
            stackalloc byte[sizeof(long)]
#endif
            , value));

        public virtual void WriteU8(byte value) => WriteBytes(
#if NETSTANDARD2_0
            new[] { value }
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
            stackalloc[] { value }
#endif
        );

        public virtual void WriteU16(ushort value) => WriteBytes(BitConverterHelper.GetBigEndianBytes(
#if NETSTANDARD2_0
            new byte[sizeof(ushort)]
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
            stackalloc byte[sizeof(ushort)]
#endif
            , value));

        public virtual void WriteU32(uint value) => WriteBytes(BitConverterHelper.GetBigEndianBytes(
#if NETSTANDARD2_0
            new byte[sizeof(uint)]
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
            stackalloc byte[sizeof(uint)]
#endif
            , value));

        public virtual void WriteU64(ulong value) => WriteBytes(BitConverterHelper.GetBigEndianBytes(
#if NETSTANDARD2_0
            new byte[sizeof(ulong)]
#elif NETSTANDARD2_1 || NET5_0_OR_GREATER
            stackalloc byte[sizeof(ulong)]
#endif
            , value));

        public virtual sbyte ReadS8() => (sbyte)ReadBytes(sizeof(byte))[0];

        public virtual short ReadS16() => BitConverterHelper.GetBigEndianInt16(ReadBytes(sizeof(short)));

        public virtual int ReadS32() => BitConverterHelper.GetBigEndianInt32(ReadBytes(sizeof(int)));

        public virtual long ReadS64() => BitConverterHelper.GetBigEndianInt64(ReadBytes(sizeof(short)));

        public virtual byte ReadU8() => ReadBytes(sizeof(byte))[0];

        public virtual ushort ReadU16() => BitConverterHelper.GetBigEndianUInt16(ReadBytes(sizeof(short)));

        public virtual uint ReadU32() => BitConverterHelper.GetBigEndianUInt32(ReadBytes(sizeof(int)));

        public virtual ulong ReadU64() => BitConverterHelper.GetBigEndianUInt64(ReadBytes(sizeof(long)));

        internal void Pad()
        {
            while (Stream.Length % 4 != 0)
                Stream.WriteByte(0);
        }

        public byte[] ToArray()
        {
            if (Stream is MemoryStream ms1)
                return ms1.ToArray();

            Stream.Position = 0;
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = Stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                return ms.ToArray();
            }
        }

        public int Length => (int)Stream.Length;

        public void Dispose()
        {
            Stream?.Dispose();
        }
    }
}
