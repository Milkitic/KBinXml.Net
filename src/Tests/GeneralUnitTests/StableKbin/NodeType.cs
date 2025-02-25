namespace StableKbin
{
    internal sealed class NodeType
    {
        public int Size { get; }

        public int Count { get; }

        public string Name { get; }

        public Converters.StringToByteDelegate GetBytes { get; }

        public Converters.ByteToStringDelegate GetString { get; }

        public NodeType(int size, int count, string name,
            Converters.StringToByteDelegate stringToByteDelegate, Converters.ByteToStringDelegate byteToStringDelegate)
        {
            Size = size;
            Count = count;
            Name = name;
            GetBytes = stringToByteDelegate;
            GetString = byteToStringDelegate;
        }
    }
}