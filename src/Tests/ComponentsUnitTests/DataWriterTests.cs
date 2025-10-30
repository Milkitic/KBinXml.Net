using System.Text;
using KbinXml.Net;
using KbinXml.Net.Internal.Writers;
using KbinXml.Net.Utils;
using Xunit.Abstractions;

namespace ComponentsUnitTests;

public class DataWriterTests
{
    private readonly ITestOutputHelper _output;

    public DataWriterTests(ITestOutputHelper output)
    {
        _output = output;
    }

    #region Byte Order Tests

    [Theory]
    [InlineData((sbyte)0, "00000000")]
    [InlineData((sbyte)1, "01000000")]
    [InlineData((sbyte)-1, "FF000000")]
    [InlineData((sbyte)127, "7F000000")]
    [InlineData((sbyte)-128, "80000000")]
    public void TestS8ByteOrder(sbyte value, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS8(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Theory]
    [InlineData((byte)0, "00000000")]
    [InlineData((byte)1, "01000000")]
    [InlineData((byte)255, "FF000000")]
    [InlineData((byte)127, "7F000000")]
    [InlineData((byte)128, "80000000")]
    public void TestU8ByteOrder(byte value, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU8(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Theory]
    [InlineData((short)0, "00000000")]
    [InlineData((short)1, "00010000")]
    [InlineData((short)-1, "FFFF0000")]
    [InlineData((short)256, "01000000")]
    [InlineData((short)32767, "7FFF0000")]
    [InlineData((short)-32768, "80000000")]
    public void TestS16ByteOrder(short value, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS16(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(4, bytes.Length);
        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Theory]
    [InlineData((ushort)0, "00000000")]
    [InlineData((ushort)1, "00010000")]
    [InlineData((ushort)256, "01000000")]
    [InlineData((ushort)65535, "FFFF0000")]
    [InlineData((ushort)32767, "7FFF0000")]
    [InlineData((ushort)32768, "80000000")]
    public void TestU16ByteOrder(ushort value, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU16(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(4, bytes.Length);
        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Theory]
    [InlineData(0, "00000000")]
    [InlineData(1, "00000001")]
    [InlineData(-1, "FFFFFFFF")]
    [InlineData(16777216, "01000000")]
    [InlineData(2147483647, "7FFFFFFF")]
    [InlineData(-2147483648, "80000000")]
    public void TestS32ByteOrder(int value, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS32(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(4, bytes.Length);
        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Theory]
    [InlineData(0u, "00000000")]
    [InlineData(1u, "00000001")]
    [InlineData(16777216u, "01000000")]
    [InlineData(4294967295u, "FFFFFFFF")]
    [InlineData(2147483647u, "7FFFFFFF")]
    [InlineData(2147483648u, "80000000")]
    public void TestU32ByteOrder(uint value, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU32(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(4, bytes.Length);
        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Theory]
    [InlineData(0L, "0000000000000000")]
    [InlineData(1L, "0000000000000001")]
    [InlineData(-1L, "FFFFFFFFFFFFFFFF")]
    [InlineData(72057594037927936L, "0100000000000000")]
    [InlineData(9223372036854775807L, "7FFFFFFFFFFFFFFF")]
    [InlineData(-9223372036854775808L, "8000000000000000")]
    public void TestS64ByteOrder(long value, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS64(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length);
        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Theory]
    [InlineData(0UL, "0000000000000000")]
    [InlineData(1UL, "0000000000000001")]
    [InlineData(72057594037927936UL, "0100000000000000")]
    [InlineData(18446744073709551615UL, "FFFFFFFFFFFFFFFF")]
    [InlineData(9223372036854775807UL, "7FFFFFFFFFFFFFFF")]
    [InlineData(9223372036854775808UL, "8000000000000000")]
    public void TestU64ByteOrder(ulong value, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU64(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length);
        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    #endregion

    #region Alignment Tests

    [Fact]
    public void TestAlignment_S8_S16_Sequence()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS8(1); // 1 byte
        writer.WriteS16(2); // 2 bytes
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length);
        Assert.Equal("01", ConvertHelper.ToHexString(bytes.Skip(0).Take(1).ToArray())); // S8 value
        Assert.Equal("000000", ConvertHelper.ToHexString(bytes.Skip(1).Take(3).ToArray())); // S8 3 bytes padding
        Assert.Equal("0002", ConvertHelper.ToHexString(bytes.Skip(4).Take(2).ToArray())); // S16 value
    }

    [Fact]
    public void TestAlignment_S8_S32_Sequence()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS8(1); // 1 byte
        writer.WriteS32(2); // 4 bytes, should be 4-byte aligned
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length); // 1 + 3 padding + 4 = 8 bytes total
        Assert.Equal("01", ConvertHelper.ToHexString(bytes.Skip(0).Take(1).ToArray())); // S8 value
        Assert.Equal("000000", ConvertHelper.ToHexString(bytes.Skip(1).Take(3).ToArray())); // 3 bytes padding
        Assert.Equal("00000002", ConvertHelper.ToHexString(bytes.Skip(4).Take(4).ToArray())); // S32 value
    }

    [Fact]
    public void TestAlignment_S16_S32_Sequence()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS16(1); // 2 bytes
        writer.WriteS32(2); // 4 bytes, should be 4-byte aligned
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length); // 2 + 2 padding + 4 = 8 bytes total
        Assert.Equal("0001", ConvertHelper.ToHexString(bytes.Skip(0).Take(2).ToArray())); // S16 value
        Assert.Equal("0000", ConvertHelper.ToHexString(bytes.Skip(2).Take(2).ToArray())); // 2 bytes padding
        Assert.Equal("00000002", ConvertHelper.ToHexString(bytes.Skip(4).Take(4).ToArray())); // S32 value
    }

    [Fact]
    public void TestAlignment_Multiple_S8_Before_S32()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS8(1); // 1 byte
        writer.WriteS8(2); // 1 byte
        writer.WriteS8(3); // 1 byte
        writer.WriteS32(4); // 4 bytes, should be 4-byte aligned
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length); // 3 + 1 padding + 4 = 8 bytes total
        Assert.Equal("010203", ConvertHelper.ToHexString(bytes.Skip(0).Take(3).ToArray())); // Three S8 values
        Assert.Equal("00", ConvertHelper.ToHexString(bytes.Skip(3).Take(1).ToArray())); // 1 byte padding
        Assert.Equal("00000004", ConvertHelper.ToHexString(bytes.Skip(4).Take(4).ToArray())); // S32 value
    }

    [Fact]
    public void TestAlignment_S32_Already_Aligned()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS32(1); // 4 bytes, already aligned
        writer.WriteS32(2); // 4 bytes, should remain aligned
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length); // 4 + 4 = 8 bytes total
        Assert.Equal("00000001", ConvertHelper.ToHexString(bytes.Skip(0).Take(4).ToArray())); // First S32
        Assert.Equal("00000002", ConvertHelper.ToHexString(bytes.Skip(4).Take(4).ToArray())); // Second S32
    }

    [Fact]
    public void TestAlignment_Complex_Sequence()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS8(1); // 1 byte
        writer.WriteS16(2); // 2 bytes
        writer.WriteS8(3); // 1 byte
        writer.WriteS32(4); // 4 bytes
        var bytes = writer.DebugGetArray();

        Assert.Equal(12, bytes.Length);
        Assert.Equal("01", ConvertHelper.ToHexString(bytes.Skip(0).Take(1).ToArray())); // S8 value
        Assert.Equal("0002", ConvertHelper.ToHexString(bytes.Skip(4).Take(2).ToArray())); // S16 value
        Assert.Equal("03", ConvertHelper.ToHexString(bytes.Skip(1).Take(1).ToArray())); // S8 value
        Assert.Equal("0000", ConvertHelper.ToHexString(bytes.Skip(6).Take(2).ToArray()));// S16 2 bytes padding
        Assert.Equal("00000004", ConvertHelper.ToHexString(bytes.Skip(8).Take(4).ToArray()));// S32 value 
    }

    [Fact]
    public void TestAlignment_Complex_Sequence_WithStringBin()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS8(1); // 1 byte
        writer.WriteBinary("E004E0D1423A4EE2".AsSpan());
        writer.WriteS16(2); // 2 bytes
        writer.WriteS8(3); // 1 byte
        writer.WriteString("Hello".AsSpan());
        writer.WriteS16(1996); // 2 bytes
        writer.WriteS32(4); // 4 bytes
        writer.WriteU8(240); // 1 byte
        var bytes = writer.DebugGetArray();

        Assert.Equal(36, bytes.Length);
        Assert.Equal("01", ConvertHelper.ToHexString(bytes.Skip(0).Take(1).ToArray())); // S8 value

        Assert.Equal("00000008", ConvertHelper.ToHexString(bytes.Skip(4).Take(4).ToArray())); // Length of bin
        Assert.Equal("E004E0D1423A4EE2", ConvertHelper.ToHexString(bytes.Skip(8).Take(8).ToArray())); // Bin value
        Assert.Equal("0002", ConvertHelper.ToHexString(bytes.Skip(16).Take(2).ToArray())); // S16 value
        Assert.Equal("03", ConvertHelper.ToHexString(bytes.Skip(1).Take(1).ToArray())); // S8 value
        Assert.Equal("00000006", ConvertHelper.ToHexString(bytes.Skip(20).Take(4).ToArray())); // Length of string
        Assert.Equal("48656C6C6F00", ConvertHelper.ToHexString(bytes.Skip(24).Take(6).ToArray())); // String value
        Assert.Equal("07CC", ConvertHelper.ToHexString(bytes.Skip(18).Take(2).ToArray())); // S16 value
        Assert.Equal("00000004", ConvertHelper.ToHexString(bytes.Skip(32).Take(4).ToArray())); // S32 value
        Assert.Equal("F0", ConvertHelper.ToHexString(bytes.Skip(2).Take(1).ToArray())); // S8 value
    }

    #endregion

    #region String Writing Tests

    [Theory]
    [InlineData("Hello", "00000006" + "48656C6C6F000000")]
    [InlineData("World", "00000006" + "576F726C64000000")]
    [InlineData("Test", "00000005" + "5465737400000000")]
    [InlineData("A", "00000002" + "41000000")]
    public void TestStringWriting_UTF8(string input, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteString(input.AsSpan());
        var bytes = writer.DebugGetArray();

        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Theory]
    [InlineData("Hello", "ASCII", "00000006" + "48656C6C6F000000")]
    [InlineData("Test", "UTF8", "00000005" + "5465737400000000")]
    [InlineData("World", "SJIS", "00000006" + "576F726C64000000")]
    public void TestStringWriting_Different_Encodings(string input, string encodingName, string expectedHex)
    {
        Encoding encoding = encodingName switch
        {
            "ASCII" => Encoding.ASCII,
            "UTF8" => Encoding.UTF8,
            "SJIS" => KnownEncodings.ShiftJIS.ToEncoding(),
            _ => Encoding.UTF8
        };
        using var writer = new DataWriter(encoding);

        writer.WriteString(input.AsSpan());
        var bytes = writer.DebugGetArray();

        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Fact]
    public void TestStringWriting_Empty_String()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteString("".AsSpan());
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length); // 4 length + 2 '\0' + 3 padding = 8 bytes total
        Assert.Equal("00000001", ConvertHelper.ToHexString(bytes.Skip(0).Take(4).ToArray())); // Length
        Assert.Equal("00", ConvertHelper.ToHexString(bytes.Skip(4).Take(1).ToArray())); // '\0'
        Assert.Equal("000000", ConvertHelper.ToHexString(bytes.Skip(5).Take(3).ToArray())); // Pad bytes
    }

    #endregion

    #region Binary Data Writing Tests

    [Theory]
    [InlineData("48656C6C6F", "0000000548656C6C6F000000")] // "Hello" in hex with length prefix
    [InlineData("DEADBEEF", "00000004DEADBEEF")]
    [InlineData("01020304", "0000000401020304")]
    public void TestBinaryWriting(string hexInput, string expectedHex)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteBinary(hexInput.AsSpan());
        var bytes = writer.DebugGetArray();

        Assert.Equal(expectedHex, ConvertHelper.ToHexString(bytes));
    }

    [Fact]
    public void TestBinaryWriting_Empty()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteBinary("".AsSpan());
        var bytes = writer.DebugGetArray();

        Assert.Equal("00000000", ConvertHelper.ToHexString(bytes)); // Just the length prefix (0)
    }

    #endregion

    #region Buffer Management Tests

    [Fact]
    public void TestBufferManagement_Mixed_Operations()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU8(1);
        writer.WriteString("Hi".AsSpan());
        writer.WriteU16(2);
        writer.WriteU32(3);
        var bytes = writer.DebugGetArray();

        Assert.True(bytes.Length > 0);
        // 验证数据包含预期的元素
        Assert.Contains((byte)1, bytes);
    }

    [Fact]
    public void TestBufferManagement_Large_Data()
    {
        using var writer = new DataWriter(Encoding.UTF8);

        // 写入大量数据以测试缓冲区扩展
        for (int i = 0; i < 1000; i++)
        {
            writer.WriteU32((uint)i);
        }

        var bytes = writer.DebugGetArray();

        Assert.Equal(4000, bytes.Length); // 1000 * 4 bytes
    }

    #endregion

    #region Boundary Condition Tests

    [Theory]
    [InlineData(sbyte.MaxValue)]
    [InlineData(sbyte.MinValue)]
    public void TestBoundaryConditions_S8_MaxMin(sbyte value)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS8(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal([(byte)value, 0, 0, 0], bytes);
    }

    [Theory]
    [InlineData(byte.MaxValue)]
    [InlineData(byte.MinValue)]
    public void TestBoundaryConditions_U8_MaxMin(byte value)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU8(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal([value, 0, 0, 0], bytes);
    }

    [Theory]
    [InlineData(short.MaxValue)]
    [InlineData(short.MinValue)]
    public void TestBoundaryConditions_S16_MaxMin(short value)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS16(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(4, bytes.Length);
    }

    [Theory]
    [InlineData(ushort.MaxValue)]
    [InlineData(ushort.MinValue)]
    public void TestBoundaryConditions_U16_MaxMin(ushort value)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU16(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(4, bytes.Length);
    }

    [Theory]
    [InlineData(int.MaxValue)]
    [InlineData(int.MinValue)]
    public void TestBoundaryConditions_S32_MaxMin(int value)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS32(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(4, bytes.Length);
    }

    [Theory]
    [InlineData(uint.MaxValue)]
    [InlineData(uint.MinValue)]
    public void TestBoundaryConditions_U32_MaxMin(uint value)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU32(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(4, bytes.Length);
    }

    [Theory]
    [InlineData(long.MaxValue)]
    [InlineData(long.MinValue)]
    public void TestBoundaryConditions_S64_MaxMin(long value)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteS64(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length);
    }

    [Theory]
    [InlineData(ulong.MaxValue)]
    [InlineData(ulong.MinValue)]
    public void TestBoundaryConditions_U64_MaxMin(ulong value)
    {
        using var writer = new DataWriter(Encoding.UTF8);

        writer.WriteU64(value);
        var bytes = writer.DebugGetArray();

        Assert.Equal(8, bytes.Length);
    }

    #endregion
}