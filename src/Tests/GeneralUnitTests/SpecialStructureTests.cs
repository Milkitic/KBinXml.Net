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
    /// Special XML Structure Tests - Tests the ability to handle special XML structures
    /// </summary>
    public class SpecialStructureTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public SpecialStructureTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        [Fact]
        public void NestedElements_PreservesStructure()
        {
            // Prepare nested XML
            var xml = @"
            <root>
                <level1>
                    <level2>
                        <level3 __type=""str"">深層ネストテスト</level3>
                    </level2>
                </level1>
            </root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify nested structure is preserved
            Assert.NotNull(result.Root.Element("level1"));
            Assert.NotNull(result.Root.Element("level1").Element("level2"));
            Assert.NotNull(result.Root.Element("level1").Element("level2").Element("level3"));
            Assert.Equal("深層ネストテスト", result.Root.Element("level1").Element("level2").Element("level3").Value);
        }

        [Fact]
        public void AttributesInNodes_PreservesAttributes()
        {
            // Prepare XML with attributes
            var xml = @"
            <root version=""1.0"">
                <node id=""1"" name=""ノード1"" __type=""str"">値1</node>
                <node id=""2"" name=""ノード2"" __type=""str"">値2</node>
            </root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify root node attributes are preserved
            Assert.Equal("1.0", result.Root.Attribute("version").Value);

            // Verify child node attributes are preserved
            var nodes = result.Root.Elements("node");
            int nodeCount = 0;

            foreach (var node in nodes)
            {
                nodeCount++;
                if (node.Attribute("id").Value == "1")
                {
                    Assert.Equal("ノード1", node.Attribute("name").Value);
                    Assert.Equal("値1", node.Value);
                }
                else if (node.Attribute("id").Value == "2")
                {
                    Assert.Equal("ノード2", node.Attribute("name").Value);
                    Assert.Equal("値2", node.Value);
                }
            }

            Assert.Equal(2, nodeCount);
        }

        [Fact]
        public void EmptyElements_PreservesStructure()
        {
            // Prepare XML with empty elements
            var xml = @"
            <root>
                <empty1 />
                <empty2></empty2>
                <notEmpty __type=""str"">コンテンツあり</notEmpty>
            </root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify empty elements are preserved
            Assert.NotNull(result.Root.Element("empty1"));
            Assert.NotNull(result.Root.Element("empty2"));
            Assert.Equal(string.Empty, result.Root.Element("empty1").Value);
            Assert.Equal(string.Empty, result.Root.Element("empty2").Value);

            // Verify non-empty elements are correct
            Assert.Equal("コンテンツあり", result.Root.Element("notEmpty").Value);
        }

        [Fact]
        public void MixedContentElements_PreservesStructure()
        {
            // Prepare XML with mixed content
            var xml = @"
            <root>
                <mixed __type=""str"">テキスト1<inner __type=""str"">内部要素</inner>テキスト2</mixed>
            </root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify mixed content structure
            var mixedElement = result.Root.Element("mixed");
            Assert.NotNull(mixedElement);

            // Note: Kbin XML conversion may not support preserving mixed content, but should ensure internal elements are normal
            Assert.NotNull(mixedElement.Element("inner"));
            Assert.Equal("内部要素", mixedElement.Element("inner").Value);
        }

        [Fact]
        public void ArrayWithDifferentTypes_PreservesStructure()
        {
            // Prepare XML with different array types
            var xml = @"
            <root>
                <numbers>
                    <int __type=""s32"">10</int>
                    <int __type=""s32"">20</int>
                    <float __type=""u32"">30</float>
                </numbers>
            </root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify array structure is preserved
            var numbers = result.Root.Element("numbers");
            Assert.NotNull(numbers);

            // Verify each element's type and value
            var intElements = numbers.Elements("int");
            var intCount = 0;
            foreach (var intElem in intElements)
            {
                intCount++;
                Assert.Equal("s32", intElem.Attribute("__type").Value);
                Assert.True(intElem.Value == "10" || intElem.Value == "20");
            }
            Assert.Equal(2, intCount);

            var floatElement = numbers.Element("float");
            Assert.NotNull(floatElement);
            Assert.Equal("u32", floatElement.Attribute("__type").Value);
            Assert.Equal("30", floatElement.Value);
        }

        [Fact]
        public void SameNameNodes_PreservesStructure()
        {
            // Prepare XML with same name nodes
            var xml = @"
            <root>
                <item __type=""str"">最初の項目</item>
                <item __type=""str"">2番目の項目</item>
                <item __type=""str"">3番目の項目</item>
            </root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify same name nodes are preserved
            var items = result.Root.Elements("item");
            var values = new[] { "最初の項目", "2番目の項目", "3番目の項目" };
            int i = 0;

            foreach (var item in items)
            {
                Assert.Equal(values[i], item.Value);
                i++;
            }

            Assert.Equal(3, i);
        }

        [Fact]
        public void ReadWriteComplex_PreservesStructure()
        {
            // Prepare complex XML
            var xml = @"
            <root version=""2.0"">
                <header>
                    <title __type=""str"">テスト文書</title>
                    <author __type=""str"">テストユーザー</author>
                    <date __type=""str"">2023-10-15</date>
                </header>
                <body>
                    <section id=""1"">
                        <title __type=""str"">セクション1</title>
                        <paragraph __type=""str"">これは最初の段落で、<emphasis __type=""str"">強調テキスト</emphasis>が含まれています。</paragraph>
                        <stats>
                            <value __type=""s32"">42</value>
                            <value __type=""s32"">-10</value>
                            <value __type=""u8"">255</value>
                        </stats>
                    </section>
                    <section id=""2"">
                        <title __type=""str"">セクション2</title>
                        <paragraph __type=""str"">これは2番目の段落です。</paragraph>
                        <list __type=""u8"" __count=""3"">1 2 3</list>
                    </section>
                </body>
                <footer>
                    <note __type=""str"">文書の終わり</note>
                </footer>
            </root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify overall structure is preserved
            Assert.Equal("2.0", result.Root.Attribute("version").Value);

            // Verify title
            Assert.Equal("テスト文書", result.Root.Element("header").Element("title").Value);
            Assert.Equal("テストユーザー", result.Root.Element("header").Element("author").Value);
            Assert.Equal("2023-10-15", result.Root.Element("header").Element("date").Value);
            Assert.Equal("str", result.Root.Element("header").Element("date").Attribute("__type").Value);

            // Verify first section
            var section1 = result.Root.Element("body").Elements("section").First(s => s.Attribute("id").Value == "1");
            Assert.Equal("セクション1", section1.Element("title").Value);
            Assert.NotNull(section1.Element("paragraph").Element("emphasis"));
            Assert.Equal("強調テキスト", section1.Element("paragraph").Element("emphasis").Value);

            var stats = section1.Element("stats").Elements("value");
            Assert.Equal(3, stats.Count());

            // Verify second section
            var section2 = result.Root.Element("body").Elements("section").First(s => s.Attribute("id").Value == "2");
            Assert.Equal("セクション2", section2.Element("title").Value);
            Assert.Equal("これは2番目の段落です。", section2.Element("paragraph").Value);

            var list = section2.Element("list");
            Assert.Equal("u8", list.Attribute("__type").Value);
            Assert.Equal("3", list.Attribute("__count").Value);
            Assert.Equal("1 2 3", list.Value);

            // Verify footer
            Assert.Equal("文書の終わり", result.Root.Element("footer").Element("note").Value);
        }

        #region Attributes and Arrays Tests

        [Fact]
        public void TestMultipleAttributes()
        {
            // Prepare XML with multiple attributes
            var xml = "<root id=\"1\" name=\"test\" value=\"123\"><node attr1=\"val1\" attr2=\"val2\" /></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify attributes are preserved
            Assert.Equal("1", result.Root.Attribute("id").Value);
            Assert.Equal("test", result.Root.Attribute("name").Value);
            Assert.Equal("123", result.Root.Attribute("value").Value);

            var node = result.Root.Element("node");
            Assert.NotNull(node);
            Assert.Equal("val1", node.Attribute("attr1").Value);
            Assert.Equal("val2", node.Attribute("attr2").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestEmptyArray()
        {
            // Prepare empty array XML
            var xml = "<root><array __type=\"s32\" __count=\"0\"></array></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify array attributes
            var array = result.Root.Element("array");
            Assert.NotNull(array);
            Assert.Equal("s32", array.Attribute("__type").Value);
            Assert.Equal("0", array.Attribute("__count").Value);
            Assert.Equal("", array.Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestLargeArray()
        {
            // Generate large array data
            var values = string.Join(" ", Enumerable.Range(1, 1000));
            var xml = $"<root><array __type=\"s32\" __count=\"1000\">{values}</array></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify array attributes and content
            var array = result.Root.Element("array");
            Assert.NotNull(array);
            Assert.Equal("s32", array.Attribute("__type").Value);
            Assert.Equal("1000", array.Attribute("__count").Value);

            // Verify array elements
            var resultValues = array.Value.Split(' ');
            Assert.Equal(1000, resultValues.Length);
            Assert.Equal("1", resultValues[0]);
            Assert.Equal("1000", resultValues[999]);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestMixedArrayTypes()
        {
            // Prepare XML with different array types
            var xml = "<root>" +
                      "<array1 __type=\"s8\" __count=\"3\">1 2 3</array1>" +
                      "<array2 __type=\"u16\" __count=\"3\">1000 2000 3000</array2>" +
                      "<array3 __type=\"s32\" __count=\"3\">-1 -2 -3</array3>" +
                      "</root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify different array types
            Assert.Equal("s8", result.Root.Element("array1").Attribute("__type").Value);
            Assert.Equal("3", result.Root.Element("array1").Attribute("__count").Value);
            Assert.Equal("1 2 3", result.Root.Element("array1").Value);

            Assert.Equal("u16", result.Root.Element("array2").Attribute("__type").Value);
            Assert.Equal("3", result.Root.Element("array2").Attribute("__count").Value);
            Assert.Equal("1000 2000 3000", result.Root.Element("array2").Value);

            Assert.Equal("s32", result.Root.Element("array3").Attribute("__type").Value);
            Assert.Equal("3", result.Root.Element("array3").Attribute("__count").Value);
            Assert.Equal("-1 -2 -3", result.Root.Element("array3").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestInvalidArrayCount()
        {
            // Prepare array count mismatch XML
            var xml = "<root><array __type=\"s32\" __count=\"3\">1 2</array></root>";

            // Set strict mode options
            var writeOptions = new WriteOptions()
            {
                StrictMode = true
            };

            // Verify throws exception
            Assert.Throws<KbinArrayCountMissMatchException>(() =>
                KbinConverter.Write(xml, KnownEncodings.UTF8, writeOptions));
        }

        [Fact]
        public void TestInvalidArrayType()
        {
            // Prepare invalid array type XML
            var xml = "<root><array __type=\"invalid\" __count=\"1\">1</array></root>";

            // Verify throws exception
            Assert.Throws<KbinTypeNotFoundException>(() =>
                KbinConverter.Write(xml, KnownEncodings.UTF8));
        }

        #endregion

        #region Special Cases Tests

        [Fact]
        public void TestEmptyNode()
        {
            // Prepare XML with empty nodes
            var xml = "<root><empty></empty><self_closing /></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify empty nodes are correctly converted
            Assert.NotNull(result.Root.Element("empty"));
            Assert.NotNull(result.Root.Element("self_closing"));
            Assert.Equal(string.Empty, result.Root.Element("empty").Value);

            var result2 = new StableKbin.XmlReader(kbin).ReadLinq();
            Assert.Equal(result2.ToString(SaveOptions.DisableFormatting), result.ToString(SaveOptions.DisableFormatting));
        }

        [Theory]
        [InlineData("<root><node __type=\"str\">特殊文字：&amp;&lt;&gt;'\"</node></root>", KnownEncodings.UTF8)]
        [InlineData("<root><node __type=\"str\">日本語テスト</node></root>", KnownEncodings.ShiftJIS)]
        public void TestSpecialCharactersAndEncoding(string xml, KnownEncodings encoding)
        {
            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, encoding);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify special characters and encoding are correctly processed
            var xmlElement = XElement.Parse(xml);
            Assert.Equal(xmlElement.Element("node").Value, result.Root.Element("node").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestDeepNestedNodes()
        {
            // Prepare deep nested XML
            var xml = "<root><a><b><c><d><e __type=\"str\">深層ネストテスト</e></d></c></b></a></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify deep nested structure is preserved
            var eElement = result.Root.Element("a").Element("b").Element("c").Element("d").Element("e");
            Assert.NotNull(eElement);
            Assert.Equal("深層ネストテスト", eElement.Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestLargeXml()
        {
            // Prepare large XML
            var builder = new StringBuilder("<root>");
            for (int i = 0; i < 100; i++)
            {
                builder.AppendFormat("<item __type=\"u32\" id=\"{0}\">{0}</item>", i);
            }
            builder.Append("</root>");
            var xml = builder.ToString();

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify large XML structure is preserved
            var items = result.Root.Elements("item").ToList();
            Assert.Equal(100, items.Count);

            // Verify certain specific elements
            Assert.Equal("0", items[0].Value);
            Assert.Equal("0", items[0].Attribute("id").Value);
            Assert.Equal("99", items[99].Value);
            Assert.Equal("99", items[99].Attribute("id").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestInvalidXmlThrowsException()
        {
            // Prepare invalid XML
            var invalidXml = "<root><unclosed>";

            // Verify throws exception
            Assert.Throws<XmlException>(() => KbinConverter.Write(invalidXml, KnownEncodings.UTF8));
        }

        #endregion
    }
}