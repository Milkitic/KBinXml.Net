using System;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net;
using Xunit;
using Xunit.Abstractions;

namespace GeneralUnitTests
{
    /// <summary>
    /// Edge Case Tests - Testing exception handling and boundary conditions
    /// </summary>
    public class EdgeCaseTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public EdgeCaseTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
#if NET8_0_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        #region Exception Tests

        [Fact]
        public void ReadXmlLinq_InvalidKbin_ThrowsKbinException()
        {
            // Prepare invalid Kbin data
            var invalidKbin = new byte[] { 0x42, 0x43, 0x44, 0x45 }; // Non-Kbin format byte array

            // Verify exception is thrown
            Assert.Throws<KbinException>(() => KbinConverter.ReadXmlLinq(invalidKbin));
        }

        [Fact]
        public void Write_InvalidEncoding_ThrowsArgumentOutOfRangeException()
        {
            // Prepare valid XML
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert XML string to XmlDocument
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Try converting with invalid encoding
            Assert.Throws<ArgumentOutOfRangeException>(() => KbinConverter.Write(xmlDoc, (KnownEncodings)999));
        }

        [Fact]
        public void Write_NullXml_ThrowsArgumentNullException()
        {
            // Try converting with null XML
            string xml = null;
            var stringException = Assert.Throws<ArgumentNullException>(() => KbinConverter.Write(xml, KnownEncodings.UTF8));
            Assert.IsType<ArgumentNullException>(stringException);
            Assert.Contains("xml", stringException.ParamName, StringComparison.OrdinalIgnoreCase);

            // Try converting with null XmlDocument
            XmlDocument xmlDoc = null;
            var docException = Assert.Throws<ArgumentNullException>(() => KbinConverter.Write(xmlDoc, KnownEncodings.UTF8));
            Assert.IsType<ArgumentNullException>(docException);
            Assert.Contains("xml", docException.ParamName, StringComparison.OrdinalIgnoreCase);

            // Try converting with null XContainer
            XContainer xContainer = null;
            var containerException = Assert.Throws<ArgumentNullException>(() => KbinConverter.Write(xContainer, KnownEncodings.UTF8));
            Assert.IsType<ArgumentNullException>(containerException);
            Assert.Contains("xml", containerException.ParamName, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public void Write_InvalidXmlString_ThrowsXmlException()
        {
            // Prepare invalid XML string (missing closing tag)
            var invalidXml = "<root><value __type=\"str\">テスト</value>";

            // Verify exception is thrown
            Assert.Throws<XmlException>(() => KbinConverter.Write(invalidXml, KnownEncodings.UTF8));
        }

        [Fact]
        public void ReadXmlBytes_InvalidKbin_ThrowsKbinException()
        {
            // Prepare invalid Kbin data
            var invalidKbin = new byte[] { 0x42, 0x43, 0x44, 0x45 }; // Non-Kbin format byte array

            // Verify exception is thrown
            Assert.Throws<KbinException>(() => KbinConverter.ReadXmlBytes(invalidKbin));
        }

        [Fact]
        public void GetXmlStream_InvalidKbin_ThrowsKbinException()
        {
            // Prepare invalid Kbin data
            var invalidKbin = new byte[] { 0x42, 0x43, 0x44, 0x45 }; // Non-Kbin format byte array

            // Verify exception is thrown
            Assert.Throws<KbinException>(() => KbinConverter.GetXmlStream(invalidKbin));
        }

        [Fact]
        public void ReadXml_InvalidKbin_ThrowsKbinException()
        {
            // Prepare invalid Kbin data
            var invalidKbin = new byte[] { 0x42, 0x43, 0x44, 0x45 }; // Non-Kbin format byte array

            // Verify exception is thrown
            Assert.Throws<KbinException>(() => KbinConverter.ReadXml(invalidKbin));
        }

        #endregion

        #region Boundary Condition Tests

        [Fact]
        public void EmptyXml_CanConvert()
        {
            // Prepare empty XML
            var xml = "<root></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify empty XML correctly converted
            Assert.NotNull(result);
            Assert.NotNull(result.Root);
            Assert.Equal("root", result.Root.Name);
            Assert.Empty(result.Root.Elements());
        }

        [Fact]
        public void LargeXml_CanConvert()
        {
            // Generate large XML
            var largeXmlBuilder = new StringBuilder();
            largeXmlBuilder.Append("<root>");

            for (int i = 0; i < 1000; i++)
            {
                largeXmlBuilder.Append($"<item id=\"{i}\" __type=\"s32\">{i}</item>");
            }

            largeXmlBuilder.Append("</root>");

            var largeXml = largeXmlBuilder.ToString();

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(largeXml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify large XML correctly converted
            Assert.NotNull(result);
            Assert.NotNull(result.Root);
            Assert.Equal(1000, result.Root.Elements("item").Count());

            // Check some specific element values
            var items = result.Root.Elements("item").ToList();
            var item0 = items.FirstOrDefault(e => e.Attribute("id")?.Value == "0");
            var item999 = items.FirstOrDefault(e => e.Attribute("id")?.Value == "999");

            Assert.NotNull(item0);
            Assert.NotNull(item999);
            Assert.Equal("0", item0.Value);
            Assert.Equal("999", item999.Value);
        }

        [Fact]
        public void DeepNestedXml_CanConvert()
        {
            // Generate deeply nested XML
            var deepXmlBuilder = new StringBuilder();
            deepXmlBuilder.Append("<root>");

            string currentTag = "<level1>";
            deepXmlBuilder.Append(currentTag);

            // Create 20 levels deep XML
            for (int i = 2; i <= 19; i++)
            {
                currentTag = $"<level{i}>";
                deepXmlBuilder.Append(currentTag);
            }

            // Add __type attribute to innermost level
            deepXmlBuilder.Append("<level20 __type=\"str\">");
            deepXmlBuilder.Append("最深部");
            deepXmlBuilder.Append("</level20>");

            // Close all tags
            for (int i = 19; i >= 1; i--)
            {
                deepXmlBuilder.Append($"</level{i}>");
            }

            deepXmlBuilder.Append("</root>");

            var deepXml = deepXmlBuilder.ToString();

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(deepXml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify deeply nested XML correctly converted
            Assert.NotNull(result);
            Assert.NotNull(result.Root);

            // Navigate to deepest level and check value
            var element = result.Root.Element("level1");
            for (int i = 2; i <= 20; i++)
            {
                element = element.Element($"level{i}");
                Assert.NotNull(element);
            }

            Assert.Equal("最深部", element.Value);
        }

        [Fact]
        public void XmlWithSpecialChars_CanConvert()
        {
            // Prepare XML with special characters
            var xml = "<root><value __type=\"str\">特殊文字: &lt;&gt;&amp;&quot;&apos;</value></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify XML with special characters correctly converted
            Assert.Equal("特殊文字: <>&\"'", result.Root.Element("value").Value);
        }

        [Fact]
        public void MaxSizeArrays_CanConvert()
        {
            // Generate an array with a large number of elements
            var arrayBuilder = new StringBuilder();
            arrayBuilder.Append("<root><array __type=\"u8\" __count=\"1000\">");

            for (int i = 0; i < 1000; i++)
            {
                arrayBuilder.Append(i % 256);
                if (i < 999) arrayBuilder.Append(" ");
            }

            arrayBuilder.Append("</array></root>");

            var arrayXml = arrayBuilder.ToString();

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(arrayXml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify array information correctly
            var array = result.Root.Element("array");
            Assert.NotNull(array);
            Assert.Equal("u8", array.Attribute("__type").Value);
            Assert.Equal("1000", array.Attribute("__count").Value);

            // Verify array content
            var values = array.Value.Split(' ');
            Assert.Equal(1000, values.Length);
        }

        [Fact]
        public void ZeroLengthData_CanConvert()
        {
            // Prepare XML with zero length data
            var xml = "<root><bin __type=\"bin\" __size=\"0\"></bin></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify zero length data correctly converted
            var bin = result.Root.Element("bin");
            Assert.NotNull(bin);
            Assert.Equal("bin", bin.Attribute("__type").Value);
            Assert.Equal("0", bin.Attribute("__size").Value);
            Assert.Equal("", bin.Value);
        }

        #endregion

        #region Exception Class Tests

        [Fact]
        public void KbinException_DefaultConstructor_CreatesInstance()
        {
            // Call default constructor
            var exception = new KbinException();

            // Verify instance is created
            Assert.NotNull(exception);
            Assert.Contains("KbinXml.Net.KbinException", exception.Message);
        }

        [Fact]
        public void KbinException_MessageConstructor_SetsMessage()
        {
            // Expected error message
            const string expectedMessage = "テストエラーメッセージ";

            // Use message constructor to create exception
            var exception = new KbinException(expectedMessage);

            // Verify message is correctly set
            Assert.Equal(expectedMessage, exception.Message);
        }

        [Fact]
        public void KbinException_InnerExceptionConstructor_SetsMessageAndInnerException()
        {
            // Prepare inner exception and message
            const string expectedMessage = "外部例外メッセージ";
            var innerException = new InvalidOperationException("内部例外メッセージ");

            // Use message and inner exception constructor to create exception
            var exception = new KbinException(expectedMessage, innerException);

            // Verify message and inner exception are correctly set
            Assert.Equal(expectedMessage, exception.Message);
            Assert.Same(innerException, exception.InnerException);
        }

        [Fact]
        public void KbinException_DerivesFromException()
        {
            // Create exception instance
            var exception = new KbinException();

            // Verify inherits from Exception
            Assert.IsAssignableFrom<Exception>(exception);
        }

        [Fact]
        public void KbinTypeNotFoundException_DerivesFromKbinException()
        {
            // Create type not found exception
            var typeException = new KbinTypeNotFoundException("test");

            // Verify inherits from KbinException
            Assert.IsAssignableFrom<KbinException>(typeException);
        }

        [Fact]
        public void KbinTypeNotFoundException_IncludesTypeNameInMessage()
        {
            // Test type name
            const string typeName = "invalidType";

            // Create exception
            var exception = new KbinTypeNotFoundException(typeName);

            // Verify type name is included in message
            Assert.Contains(typeName, exception.Message);
        }

        #endregion

        #region Span Value Tests

        [Fact]
        public void TestLargeXmlWithManyNodes()
        {
            // 构造大量节点，强制 XmlReader 内部缓冲区重用
            var xml = $"""
                      <root>
                          {string.Join("\n", Enumerable.Range(1, 1000).Select(k => $"<item __type='str'>Value{k}</item>"))}
                      </root>
                      """;
            var xElement = XElement.Parse(xml);

            var result = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result2 = KbinConverter.Write(xElement, KnownEncodings.UTF8);
            Assert.Equal(result, result2);
        }

        [Fact]
        public void TestLongAttributeValues()
        {
            // 测试长属性值是否会导致缓冲区重用
            var xml = $"""
                       <root>
                           <item __type='str' attr1='{new string('a', 10000)}'>{new string('b', 10000)}</item>
                       </root>
                       """;
            var xElement = XElement.Parse(xml);

            var result = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result2 = KbinConverter.Write(xElement, KnownEncodings.UTF8);
            Assert.Equal(result, result2);
        }

        [Fact]
        public void TestMixedNodeTypes()
        {
            // 测试混合节点类型（属性、文本、嵌套）
            var xml = """
                      <root>
                          <a __type='s32' __count='2'>1 2</a>
                          <b __type='str'>Text</b>
                          <c attr='value' __type='u8'>255</c>
                      </root>
                      """;
            var xElement = XElement.Parse(xml);

            var result = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result2 = KbinConverter.Write(xElement, KnownEncodings.UTF8);
            Assert.Equal(result, result2);
        }

        #endregion
    }
}
