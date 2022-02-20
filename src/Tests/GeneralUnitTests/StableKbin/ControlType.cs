namespace StableKbin
{
    internal enum ControlType : byte
    {
        NodeStart = 0x01,
        Attribute = 0x2E,
        NodeEnd   = 0xBE,
        FileEnd   = 0xBF
    }
}
