using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using KbinXml.Utils;

namespace KbinXml.Writers
{
    public class BeBinaryWriter
    {
        protected readonly List<byte> Stream;
        public BeBinaryWriter()
        {
            Stream = new List<byte>(0);
        }

        public virtual void WriteBytes(ReadOnlySpan<byte> buffer)
        {
            Stream.Capacity += buffer.Length;
            foreach (var span in buffer)
            {
                Stream.Add(span);
            }
        }

        public virtual void WriteS8(sbyte value)
        {
            Stream.Add((byte)value);
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
            Stream.Add(value);
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
            var add = Stream.Count % 4;
            if (add == 0) return;

            var left = 4 - add;
            Stream.Capacity += left;
            for (int i = 0; i < left; i++)
            {
                Stream.Add(0);
            }
        }

        public Span<byte> AsSpan()
        {
#if NET5_0_OR_GREATER
            return CollectionsMarshal.AsSpan(Stream);
#else
            return Stream.ToArray().AsSpan();
#endif
        }
    }
}
