namespace KbinXml.Internal;

internal class NodeType
{
    public int Size { get; }

    public int Count { get; }

    public string Name { get; }

    public WriteStringDelegate WriteString { get; }

    public ByteToStringDelegate GetString { get; }

    public NodeType(int size, int count, string name,
        WriteStringDelegate writeStringDelegate, ByteToStringDelegate byteToStringDelegate)
    {
        Size = size;
        Count = count;
        Name = name;
        WriteString = writeStringDelegate;
        GetString = byteToStringDelegate;
    }
}