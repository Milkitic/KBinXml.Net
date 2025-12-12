#if NET8_0_OR_GREATER
using System;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Readers;

internal ref partial struct BigEndianReader
{
    public ValueReadResult<T> Read<T>() where T : System.Numerics.IBinaryInteger<T>, System.Numerics.IMinMaxValue<T>
    {
        var result = ReadBytes(T.Zero.GetByteCount());
        return new ValueReadResult<T>
        (
            BitConverterHelper.ToBe<T>(result.Span)
#if USELOG
            , result.ReadStatus
#endif
        );
    }

    public ValueReadResult<sbyte> ReadS8() => Read<sbyte>();
    public ValueReadResult<short> ReadS16() => Read<short>();
    public ValueReadResult<int> ReadS32() => Read<int>();
    public ValueReadResult<long> ReadS64() => Read<long>();
    public ValueReadResult<byte> ReadU8() => Read<byte>();
    public ValueReadResult<ushort> ReadU16() => Read<ushort>();
    public ValueReadResult<uint> ReadU32() => Read<uint>();
    public ValueReadResult<ulong> ReadU64() => Read<ulong>();
}
#endif