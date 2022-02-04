using System;
using System.IO;
using KbinXml.Utils;

namespace KbinXml.Writers
{
    public class BeBinaryWriter
    {
        protected internal readonly MemoryStream Stream;
        public BeBinaryWriter()
        {
            Stream = new MemoryStream(0);
        }

        public virtual void WriteBytes(ReadOnlySpan<byte> buffer)
        {
            Stream.WriteSpan(buffer);
        }

        public virtual void WriteS8(sbyte value)
        {
            WriteBytes(stackalloc[] { (byte)value });
        }

        public virtual void WriteS16(short value)
        {
            BitConverterHelper.WriteBeBytes(Stream, value);
        }

        public virtual void WriteS32(int value)
        {
            BitConverterHelper.WriteBeBytes(Stream, value);
        }

        public virtual void WriteS64(long value)
        {
            BitConverterHelper.WriteBeBytes(Stream, value);
        }

        public virtual void WriteU8(byte value)
        {
            Stream.WriteByte(value);
        }

        public virtual void WriteU16(ushort value)
        {
            BitConverterHelper.WriteBeBytes(Stream, value);
        }

        public virtual void WriteU32(uint value)
        {
            BitConverterHelper.WriteBeBytes(Stream, value);
        }

        public virtual void WriteU64(ulong value)
        {
            BitConverterHelper.WriteBeBytes(Stream, value);
        }

        internal void Pad()
        {
            while (Stream.Length % 4 != 0)
                Stream.WriteByte(0);
        }

        public byte[] ToArray()
        {
            return Stream.ToArray();
        }
    }
}
