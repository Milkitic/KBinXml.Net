using System.Text;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Writers;

public class NodeWriter : BeBinaryWriter
{
    public bool Compressed { get; }
    private readonly Encoding _encoding;

    public NodeWriter(bool compressed, Encoding encoding)
    {
        Compressed = compressed;
        _encoding = encoding;
    }

    public void WriteString(string value)
    {
        if (Compressed)
        {
            WriteU8((byte)value.Length);
            SixbitHelper.EncodeAndWrite(Stream, value);
        }
        else
        {
            WriteU8((byte)((value.Length - 1) | (1 << 6)));
            WriteBytes(_encoding.GetBytes(value));
        }
    }
}