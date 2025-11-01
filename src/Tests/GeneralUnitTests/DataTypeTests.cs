using System;
using System.Text;
using System.Xml.Linq;
using KbinXml.Net;
using KbinXml.Net.Internal.TypeConverters;
using KbinXml.Net.Utils;
using Xunit;
using Xunit.Abstractions;

namespace GeneralUnitTests
{
    /// <summary>
    /// Data Type Tests - Test conversion of various data types
    /// </summary>
    public class DataTypeTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public DataTypeTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        #region Numeric Type Tests

        [Theory]
        [InlineData("u8", "0", 0)]
        [InlineData("u8", "127", 127)]
        [InlineData("u8", "255", 255)]
        public void NumericTypeU8_ConversionIsCorrect(string type, string value, byte expected)
        {
            TestNumericTypeConversion(type, value);
        }

        [Theory]
        [InlineData("s8", "0", 0)]
        [InlineData("s8", "-128", -128)]
        [InlineData("s8", "127", 127)]
        public void NumericTypeS8_ConversionIsCorrect(string type, string value, sbyte expected)
        {
            TestNumericTypeConversion(type, value);
        }

        [Theory]
        [InlineData("u16", "0", 0U)]
        [InlineData("u16", "32767", 32767U)]
        [InlineData("u16", "65535", 65535U)]
        public void NumericTypeU16_ConversionIsCorrect(string type, string value, ushort expected)
        {
            TestNumericTypeConversion(type, value);
        }

        [Theory]
        [InlineData("s16", "0", 0)]
        [InlineData("s16", "-32768", -32768)]
        [InlineData("s16", "32767", 32767)]
        public void NumericTypeS16_ConversionIsCorrect(string type, string value, short expected)
        {
            TestNumericTypeConversion(type, value);
        }

        [Theory]
        [InlineData("u32", "0", 0U)]
        [InlineData("u32", "2147483647", 2147483647U)]
        [InlineData("u32", "4294967295", 4294967295U)]
        public void NumericTypeU32_ConversionIsCorrect(string type, string value, uint expected)
        {
            TestNumericTypeConversion(type, value);
        }

        [Theory]
        [InlineData("s32", "0", 0)]
        [InlineData("s32", "-2147483648", -2147483648)]
        [InlineData("s32", "2147483647", 2147483647)]
        public void NumericTypeS32_ConversionIsCorrect(string type, string value, int expected)
        {
            TestNumericTypeConversion(type, value);
        }

        [Theory]
        [InlineData("u64", "0", "0")]
        [InlineData("u64", "9223372036854775807", "9223372036854775807")]
        [InlineData("u64", "18446744073709551615", "18446744073709551615")]
        public void NumericTypeU64_ConversionIsCorrect(string type, string value, string expected)
        {
            TestNumericTypeConversion(type, value);
        }

        [Theory]
        [InlineData("s64", "0", "0")]
        [InlineData("s64", "-9223372036854775808", "-9223372036854775808")]
        [InlineData("s64", "9223372036854775807", "9223372036854775807")]
        public void NumericTypeS64_ConversionIsCorrect(string type, string value, string expected)
        {
            TestNumericTypeConversion(type, value);
        }

        private void TestNumericTypeConversion(string type, string value)
        {
            // Prepare XML
            var xml = $"<root><value __type=\"{type}\">{value}</value></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify value is unchanged
            Assert.Equal(value, result.Root.Element("value").Value);

            // Verify type attribute is preserved
            Assert.Equal(type, result.Root.Element("value").Attribute("__type").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        #endregion

        #region String Type Tests

        [Theory]
        [InlineData("Hello World")]
        [InlineData("テスト文字列")]
        [InlineData("Special chars")]
        [InlineData("")]
        public void StringType_ConversionIsCorrect(string value)
        {
            // Prepare XML
            var xml = $"<root><value __type=\"str\">{value}</value></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify value is unchanged
            Assert.Equal(value, result.Root.Element("value").Value);

            // Verify type attribute is preserved
            Assert.Equal("str", result.Root.Element("value").Attribute("__type").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        #endregion

        #region Binary Type Tests

        [Theory]
        [InlineData("deadbeef", 4)]
        [InlineData("0123456789abcdef", 8)]
        [InlineData("", 0)]
        public void BinaryType_ConversionIsCorrect(string hexValue, int size)
        {
            // Prepare XML
            var xml = $"<root><value __type=\"bin\" __size=\"{size}\">{hexValue}</value></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify value is unchanged (ignoring case)
            Assert.Equal(hexValue.ToUpperInvariant(), result.Root.Element("value").Value.ToUpperInvariant());

            // Verify type attribute and size attribute are preserved
            Assert.Equal("bin", result.Root.Element("value").Attribute("__type").Value);
            Assert.Equal(size.ToString(), result.Root.Element("value").Attribute("__size").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        #endregion

        #region Array Type Tests

        [Theory]
        [InlineData("s8", "0 1 -1 127 -128", 5)]
        [InlineData("u16", "0 65535 1 2 3", 5)]
        [InlineData("s32", "-2147483648 0 2147483647", 3)]
        public void ArrayType_ConversionIsCorrect(string type, string values, int count)
        {
            // Prepare XML
            var xml = $"<root><array __type=\"{type}\" __count=\"{count}\">{values}</array></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify value is unchanged
            Assert.Equal(values, result.Root.Element("array").Value);

            // Verify type attribute and count attribute are preserved
            Assert.Equal(type, result.Root.Element("array").Attribute("__type").Value);
            Assert.Equal(count.ToString(), result.Root.Element("array").Attribute("__count").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        #endregion

        #region IP4 Type Tests

        [Fact]
        public void Ip4Type_ConversionIsCorrect()
        {
            // Prepare XML
            var xml = "<root><ip __type=\"ip4\">192.168.1.1</ip></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify value is unchanged
            Assert.Equal("192.168.1.1", result.Root.Element("ip").Value);

            // Verify type attribute is preserved
            Assert.Equal("ip4", result.Root.Element("ip").Attribute("__type").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        #endregion

        #region Type Converter Basic Tests

        [Fact]
        public void ByteConverterTest()
        {
            byte value = 123;
            string valueStr = value.ToString();

            // Test writing
            var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
            int bytesWritten = U8Converter.Instance.WriteString(ref builder, valueStr.AsSpan());

            Assert.Equal(1, bytesWritten);

            // Test reading
            var bytes = builder.AsSpan().ToArray();
            string result = U8Converter.Instance.ToString(bytes);

            Assert.Equal(valueStr, result);
        }

        [Fact]
        public void Int32ConverterTest()
        {
            int value = 123456789;
            string valueStr = value.ToString();

            // Test writing
            var builder = new ValueListBuilder<byte>(stackalloc byte[8]);
            int bytesWritten = S32Converter.Instance.WriteString(ref builder, valueStr.AsSpan());

            Assert.Equal(4, bytesWritten);

            // Test reading
            var bytes = builder.AsSpan().ToArray();
            string result = S32Converter.Instance.ToString(bytes);

            Assert.Equal(valueStr, result);
        }

        [Fact]
        public void Ip4ConverterTest()
        {
            string value = "192.168.1.1";

            // Test writing
            var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
            int bytesWritten = Ip4Converter.Instance.WriteString(ref builder, value.AsSpan());

            Assert.Equal(4, bytesWritten);

            // Test reading
            var bytes = builder.AsSpan().ToArray();
            string result = Ip4Converter.Instance.ToString(bytes);

            Assert.Equal(value, result);
        }

        #endregion

        #region Exception Tests

        [Fact]
        public void InvalidValue_ThrowsException()
        {
            // Prepare invalid value XML (out of type range)
            var xml = "<root><value __type=\"u8\">256</value></root>";

            // Verify throws exception
            Assert.Throws<KbinException>(() => KbinConverter.Write(xml, KnownEncodings.UTF8));
        }

        [Fact]
        public void InvalidType_ThrowsException()
        {
            // Prepare invalid type XML
            var xml = "<root><value __type=\"invalid_type\">123</value></root>";

            // Verify throws exception
            Assert.Throws<KbinTypeNotFoundException>(() => KbinConverter.Write(xml, KnownEncodings.UTF8));
        }

        #endregion

        #region Primitive Type Conversion Tests

        [Theory]
        [ClassData(typeof(ByteTestData))]
        public void ByteTest(byte value)
        {
            DoWorks(value, x => StableKbin.Converters.U8ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.U8ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    U8Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => U8Converter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(SbyteTestData))]
        public void SbyteTest(sbyte value)
        {
            DoWorks(value, x => StableKbin.Converters.S8ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.S8ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    S8Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => S8Converter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(Int16TestData))]
        public void Int16Test(short value)
        {
            DoWorks(value, x => StableKbin.Converters.S16ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.S16ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    S16Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => S16Converter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(Int32TestData))]
        public void Int32Test(int value)
        {
            DoWorks(value, x => StableKbin.Converters.S32ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.S32ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    S32Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => S32Converter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(Int64TestData))]
        public void Int64Test(long value)
        {
            DoWorks(value, x => StableKbin.Converters.S64ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.S64ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    S64Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => S64Converter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(UInt16TestData))]
        public void UInt16Test(ushort value)
        {
            DoWorks(value, x => StableKbin.Converters.U16ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.U16ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    U16Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => U16Converter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(UInt32TestData))]
        public void UInt32Test(uint value)
        {
            DoWorks(value, x => StableKbin.Converters.U32ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.U32ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    U32Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => U32Converter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(UInt64TestData))]
        public void UInt64Test(ulong value)
        {
            DoWorks(value, x => StableKbin.Converters.U64ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.U64ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    U64Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => U64Converter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(SingleTestData))]
        public void SingleTest(float value)
        {
            DoWorks(value, x => StableKbin.Converters.SingleToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.SingleToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    FloatConverter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => FloatConverter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(DoubleTestData))]
        public void DoubleTest(double value)
        {
            DoWorks(value, x => StableKbin.Converters.DoubleToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.DoubleToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    DoubleConverter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => DoubleConverter.Instance.ToString(bytes));
        }

        [Theory]
        [ClassData(typeof(Ip4TestData))]
        public void Ip4Test(string value)
        {
            DoWorks(value, x => StableKbin.Converters.Ip4ToBytes(x.AsSpan()).ToArray(),
                x => StableKbin.Converters.Ip4ToString(x),
                str =>
                {
                    var builder = new ValueListBuilder<byte>(stackalloc byte[4]);
                    Ip4Converter.Instance.WriteString(ref builder, str.AsSpan());
                    return builder.AsSpan().ToArray();
                }, bytes => Ip4Converter.Instance.ToString(bytes));
        }

        private static void DoWorks(object value,
            Func<string, byte[]> toBytesOld,
            Func<byte[], string> toStringOld,
            Func<string, byte[]> toBytesNew,
            Func<byte[], string> toStringNew)
        {
            var str = value.ToString();

            var bytes = toBytesOld(str);
            var bytes2 = toBytesNew(str);

            var output = toStringOld(bytes);
            var output2 = toStringNew(bytes2);

            Assert.Equal(bytes, bytes2);
            Assert.Equal(output, output2);
        }

        #endregion
    }
}