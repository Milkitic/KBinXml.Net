using System;
using System.Buffers;
using System.Text;
using KbinXml.Net.Internal;
using U8Xml;

namespace KbinXml.Net.Writers;

public class Utf8DataWriter : DataWriter
{
    public Utf8DataWriter() : base(Encoding.UTF8)
    {
    }

    public void WriteUtf8String(in RawString text)
    {
        var length = text.Length + 1;
        byte[]? arr = null;
        Span<byte> span = length <= Constants.MaxStackLength
            ? stackalloc byte[length]
            : arr = ArrayPool<byte>.Shared.Rent(length);
        try
        {
            if (arr != null) span = span.Slice(0, length);
            text.AsSpan().CopyTo(span);

            WriteU32((uint)length);
            Write32BitAligned(span);
        }
        finally
        {
            if (arr != null) ArrayPool<byte>.Shared.Return(arr);
        }
    }
}