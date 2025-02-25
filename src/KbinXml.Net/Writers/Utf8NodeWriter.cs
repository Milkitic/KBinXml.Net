using System.Runtime.CompilerServices;
using System.Text;
using U8Xml;

namespace KbinXml.Net.Writers;

public class Utf8NodeWriter : NodeWriter
{
    public Utf8NodeWriter(bool compressed) : base(compressed, Encoding.UTF8)
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void WriteUtf8String(in RawString value)
    {
        if (Compressed)
        {
            WriteString(value.ToString());
        }
        else
        {
            var charCount = value.GetCharCount();
            WriteU8((byte)((charCount - 1) | (1 << 6)));
            WriteBytes(value.AsSpan());
        }
    }
}