using System;
using System.IO;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Writers
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
            Span<byte> span = stackalloc byte[sizeof(short)];
            var builder = new ValueListBuilder<byte>(span);
            BitConverterHelper.WriteBeBytes(ref builder, value);
            WriteBytes(builder.AsSpan());
        }

        public virtual void WriteS32(int value)
        {
            Span<byte> span = stackalloc byte[sizeof(int)];
            var builder = new ValueListBuilder<byte>(span);
            BitConverterHelper.WriteBeBytes(ref builder, value);
            WriteBytes(builder.AsSpan());
        }

        public virtual void WriteS64(long value)
        {
            Span<byte> span = stackalloc byte[sizeof(long)];
            var builder = new ValueListBuilder<byte>(span);
            BitConverterHelper.WriteBeBytes(ref builder, value);
            WriteBytes(builder.AsSpan());
        }

        public virtual void WriteU8(byte value)
        {
            Stream.WriteByte(value);
        }

        public virtual void WriteU16(ushort value)
        {
            Span<byte> span = stackalloc byte[sizeof(ushort)];
            var builder = new ValueListBuilder<byte>(span);
            BitConverterHelper.WriteBeBytes(ref builder, value);
            WriteBytes(builder.AsSpan());
        }

        public virtual void WriteU32(uint value)
        {
            Span<byte> span = stackalloc byte[sizeof(uint)];
            var builder = new ValueListBuilder<byte>(span);
            BitConverterHelper.WriteBeBytes(ref builder, value);
            WriteBytes(builder.AsSpan());
        }

        public virtual void WriteU64(ulong value)
        {
            Span<byte> span = stackalloc byte[sizeof(ulong)];
            var builder = new ValueListBuilder<byte>(span);
            BitConverterHelper.WriteBeBytes(ref builder, value);
            WriteBytes(builder.AsSpan());
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
