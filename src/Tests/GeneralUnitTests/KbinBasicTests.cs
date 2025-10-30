using System;
using System.IO;
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
    /// Kbin Basic Function Tests - Test the basic read/write functionality of KbinConverter
    /// </summary>
    public class KbinBasicTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public KbinBasicTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        #region Basic Reading Tests

        [Fact]
        public void ReadXmlLinq_ValidKbin_ReturnsXDocument()
        {
            // Prepare valid XML
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert to Kbin
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);

            // Read XDocument from Kbin
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify results
            Assert.NotNull(result);
            Assert.NotNull(result.Root);
            Assert.Equal("root", result.Root.Name.ToString());
            Assert.Equal("テスト", result.Root.Element("value").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void ReadXmlLinq_WithEncodingOutput_ReturnsEncodingAndXDocument()
        {
            // Prepare valid XML
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert to Kbin
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);

            // Read XDocument from Kbin and get encoding information
            KnownEncodings encoding;
            var result = KbinConverter.ReadXmlLinq(kbin, out encoding);

            // Verify results
            Assert.NotNull(result);
            Assert.Equal(KnownEncodings.UTF8, encoding);
            Assert.Equal("テスト", result.Root.Element("value").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void ReadXmlBytes_ValidKbin_ReturnsXmlBytes()
        {
            // Prepare valid XML
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert to Kbin
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);

            // Read XML bytes from Kbin
            var xmlBytes = KbinConverter.ReadXmlBytes(kbin);

            // Verify results
            Assert.NotNull(xmlBytes);
            Assert.True(xmlBytes.Length > 0);

            // Convert bytes back to string and verify content
            var resultStr = Encoding.UTF8.GetString(xmlBytes);
            Assert.Contains("<root>", resultStr);
            Assert.Contains("<value", resultStr);
            Assert.Contains("テスト", resultStr);
            Assert.Contains("</root>", resultStr);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(xml.Trim(), formatXml);
        }

        [Fact]
        public void GetXmlStream_ValidKbin_ReturnsMemoryStream()
        {
            // Prepare valid XML
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert to Kbin
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);

            // Get MemoryStream from Kbin
            using var stream = KbinConverter.GetXmlStream(kbin);

            // Verify results
            Assert.NotNull(stream);
            Assert.True(stream.Length > 0);

            // Read stream and verify content
            stream.Position = 0;
            using var reader = new StreamReader(stream, Encoding.UTF8, true);
            var resultStr = reader.ReadToEnd();
            Assert.Contains("<root>", resultStr);
            Assert.Contains("<value", resultStr);
            Assert.Contains("テスト", resultStr);
            Assert.Contains("</root>", resultStr);
        }

        [Fact]
        public void ReadXml_ValidKbin_ReturnsXmlDocument()
        {
            // Prepare valid XML
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert to Kbin
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);

            // Read XmlDocument from Kbin
            var result = KbinConverter.ReadXml(kbin);

            // Verify results
            Assert.NotNull(result);
            Assert.NotNull(result.DocumentElement);
            Assert.Equal("root", result.DocumentElement.Name);
            Assert.Equal("テスト", result.DocumentElement.SelectSingleNode("value").InnerText);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(xml, formatXml);
        }

        #endregion

        #region Basic Writing Tests

        [Fact]
        public void Write_XmlDocument_ReturnsKbinBytes()
        {
            // Prepare XmlDocument
            var doc = new XmlDocument();
            var xml = "<root><value __type=\"str\">テスト</value></root>";
            doc.LoadXml(xml);

            // Convert to Kbin
            var kbin = KbinConverter.Write(doc, KnownEncodings.UTF8);

            // Verify results
            Assert.NotNull(kbin);
            Assert.True(kbin.Length > 0);

            // Verify it can be read back
            var result = KbinConverter.ReadXmlLinq(kbin);
            Assert.Equal("テスト", result.Root.Element("value").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void Write_XContainer_ReturnsKbinBytes()
        {
            // Prepare XDocument
            var doc = new XDocument(
                new XElement("root",
                    new XElement("value", new XAttribute("__type", "str"), "テスト")
                )
            );

            // Convert to Kbin
            var kbin = KbinConverter.Write(doc, KnownEncodings.UTF8);

            // Verify results
            Assert.NotNull(kbin);
            Assert.True(kbin.Length > 0);

            // Verify it can be read back
            var result = KbinConverter.ReadXmlLinq(kbin);
            Assert.Equal("テスト", result.Root.Element("value").Value);
        }

        [Fact]
        public void Write_XmlString_ReturnsKbinBytes()
        {
            // Prepare XML string
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert to Kbin
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);

            // Verify results
            Assert.NotNull(kbin);
            Assert.True(kbin.Length > 0);

            // Verify it can be read back
            var result = KbinConverter.ReadXmlLinq(kbin);
            Assert.Equal("テスト", result.Root.Element("value").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void Write_XmlBytes_ReturnsKbinBytes()
        {
            // Prepare XML bytes
            var xmlStr = "<root><value __type=\"str\">テスト</value></root>";
            var xmlBytes = Encoding.UTF8.GetBytes(xmlStr);

            // Convert to Kbin
            var kbin = KbinConverter.Write(xmlBytes, KnownEncodings.UTF8);

            // Verify results
            Assert.NotNull(kbin);
            Assert.True(kbin.Length > 0);

            // Verify it can be read back
            var result = KbinConverter.ReadXmlLinq(kbin);
            Assert.Equal("テスト", result.Root.Element("value").Value);

            Assert.Equal(xmlStr, result.ToString(SaveOptions.DisableFormatting));
        }

        #endregion

        #region Read-Write Combined Tests

        [Fact]
        public void ReadWriteSimple_PreservesContent()
        {
            // Prepare simple XML
            var xml = "<root><value __type=\"str\">Simple Test</value><version __type=\"str\">1.0</version></root>";

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify content consistency
            Assert.Equal("Simple Test", result.Root.Element("value").Value);
            Assert.Equal("1.0", result.Root.Element("version").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void ReadWriteWithAttributes_PreservesAttributes()
        {
            // Prepare XML with attributes
            var xml = "<root><item __type=\"str\" enabled=\"true\" id=\"1\" name=\"テスト項目\">属性付きアイテム</item></root>";

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify attribute consistency
            var itemElement = result.Root.Element("item");
            Assert.Equal("1", itemElement.Attribute("id").Value);
            Assert.Equal("テスト項目", itemElement.Attribute("name").Value);
            Assert.Equal("true", itemElement.Attribute("enabled").Value);
            Assert.Equal("属性付きアイテム", itemElement.Value);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(formatXml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void ReadWriteNestedElements_PreservesStructure()
        {
            // Prepare nested XML
            var xml = @"
            <root>
                <parent>
                    <child1 __type=""str"">値1</child1>
                    <child2 __type=""str"">値2</child2>
                    <subParent>
                        <subChild __type=""str"">ネスト値</subChild>
                    </subParent>
                </parent>
            </root>";

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify nested structure consistency
            var parentElement = result.Root.Element("parent");
            Assert.Equal("値1", parentElement.Element("child1").Value);
            Assert.Equal("値2", parentElement.Element("child2").Value);
            Assert.Equal("ネスト値", parentElement.Element("subParent").Element("subChild").Value);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(formatXml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void ReadWriteElementArray_PreservesArray()
        {
            // Prepare array XML
            var xml = @"
            <root>
                <items>
                    <item __type=""str"">最初の項目</item>
                    <item __type=""str"">2番目の項目</item>
                    <item __type=""str"">3番目の項目</item>
                </items>
            </root>";

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify array consistency
            var itemElements = result.Root.Element("items").Elements("item").ToList();
            Assert.Equal(3, itemElements.Count);
            Assert.Equal("最初の項目", itemElements[0].Value);
            Assert.Equal("2番目の項目", itemElements[1].Value);
            Assert.Equal("3番目の項目", itemElements[2].Value);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(formatXml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void ReadWriteWithAttributesAndElements_PreservesAll()
        {
            // Prepare complex XML (mixed attributes and elements)
            var xml = @"
            <config version=""1.0"">
                <server id=""main"" port=""8080"">
                    <name __type=""str"">メインサーバー</name>
                    <services>
                        <service __type=""str"" type=""http"">Webサービス</service>
                        <service __type=""str"" type=""ftp"">ファイルサービス</service>
                    </services>
                </server>
            </config>";

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify complex structure consistency
            Assert.Equal("1.0", result.Root.Attribute("version").Value);

            var serverElement = result.Root.Element("server");
            Assert.Equal("main", serverElement.Attribute("id").Value);
            Assert.Equal("8080", serverElement.Attribute("port").Value);
            Assert.Equal("メインサーバー", serverElement.Element("name").Value);

            var serviceElements = serverElement.Element("services").Elements("service").ToList();
            Assert.Equal(2, serviceElements.Count);
            Assert.Equal("http", serviceElements[0].Attribute("type").Value);
            Assert.Equal("Webサービス", serviceElements[0].Value);
            Assert.Equal("ftp", serviceElements[1].Attribute("type").Value);
            Assert.Equal("ファイルサービス", serviceElements[1].Value);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(formatXml, result.ToString(SaveOptions.DisableFormatting));
        }

        #endregion

        #region Options Tests

        [Fact]
        public void WriteOptions_CustomPrefix_HandlesCorrectly()
        {
            // Prepare XML
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Prepare custom prefix WriteOptions
            var options = new WriteOptions
            {
                Compress = true,
                RepairedPrefix = "custom_"
            };

            // Use custom prefix to convert
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8, options);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify value correctness
            Assert.Equal("テスト", result.Root.Element("value").Value);

            Assert.Equal(xml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Theory]
        [InlineData(KnownEncodings.ASCII)]
        [InlineData(KnownEncodings.EUC_JP)]
        [InlineData(KnownEncodings.ShiftJIS)]
        [InlineData(KnownEncodings.UTF8)]
        public void ReadWrite_DifferentEncodings_PreservesData(KnownEncodings encoding)
        {
            // Prepare test text suitable for current encoding
            string testText;
            switch (encoding)
            {
                case KnownEncodings.UTF8:
                    testText = "UTF-8テスト";
                    break;
                case KnownEncodings.ASCII:
                    testText = "ASCII test";
                    break;
                case KnownEncodings.ShiftJIS:
                    testText = "ShiftJISテスト";
                    break;
                case KnownEncodings.EUC_JP:
                    testText = "EUC-JPテスト";
                    break;
                default:
                    testText = "Default test";
                    break;
            }

            // Prepare XML
            var xml = $"<root><value __type=\"str\">{testText}</value></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, encoding);
            var resultDoc = KbinConverter.ReadXmlLinq(kbin);

            // Verify text and structure preservation
            Assert.Equal(testText, resultDoc.Root.Element("value").Value);

            Assert.Equal(xml, resultDoc.ToString(SaveOptions.DisableFormatting));
        }

        #endregion

        #region Special Test Cases

        [Fact]
        public void TestSpecialXml_NestedElements()
        {
            var xml = """
                    <confuse__a3>
                      <confuse__a4>
                        <confuse__a5 __type="s32">0</confuse__a5>
                        <confuse__a6 __type="s8">0</confuse__a6>
                      </confuse__a4>
                      <confuse__a4>
                        <confuse__a5 __type="s32">1</confuse__a5>
                        <confuse__a6 __type="s8">0</confuse__a6>
                      </confuse__a4>
                    </confuse__a3>
                    """;

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify content consistency - we verify specific values and structure
            var confuseElements = result.Root.Elements("confuse__a4").ToList();
            Assert.Equal(2, confuseElements.Count);

            Assert.Equal("0", confuseElements[0].Element("confuse__a5").Value);
            Assert.Equal("0", confuseElements[0].Element("confuse__a6").Value);

            Assert.Equal("1", confuseElements[1].Element("confuse__a5").Value);
            Assert.Equal("0", confuseElements[1].Element("confuse__a6").Value);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(formatXml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestJapaneseStrings()
        {
            var xml = """
                    <card id="5502">
                        <info>
                            <texture __type="str">ap_06_R0002</texture>
                            <title __type="str">ボルテ10周年★記念限定カード</title>
                            <message_a __type="str">はわわ～！ボルテはついニ[br:0]10周年を迎えまシタ～♪</message_a>
                            <message_b __type="str">10周年…[br:0]大きな節目を迎えたわね！</message_b>
                            <message_c __type="str">∩ ^-^ )∩わーい！[br:0]∩・v・)∩お祝いしよー！</message_c>
                            <message_d __type="str">わわワ～！にぎゃかですッ！[br:0]全員集合ですョーッ！</message_d>
                            <message_e __type="str">Foo！記念すべき日に[br:0]先生も燃えているゾッ★</message_e>
                            <message_f __type="str">魂たちに連れまわされて[br:0]眠気が…、オヤスミ…。</message_f>
                            <message_g __type="str">料理はまだまだあるよ！[br:0]おにーちゃん♪</message_g>
                            <message_h __type="str">*△ これからもボルテを！[br:0]†△ 宜しくお願いします。</message_h>
                        </info>
                    </card>
                    """;

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.ShiftJIS);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify Japanese text correctly converted
            Assert.Equal("ap_06_R0002", result.Root.Element("info").Element("texture").Value);
            Assert.Equal("ボルテ10周年★記念限定カード", result.Root.Element("info").Element("title").Value);
            Assert.Equal("はわわ～！ボルテはついニ[br:0]10周年を迎えまシタ～♪", result.Root.Element("info").Element("message_a").Value);
            Assert.Equal("10周年…[br:0]大きな節目を迎えたわね！", result.Root.Element("info").Element("message_b").Value);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(formatXml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestBinaryData()
        {
            var xml = """
                    <kingdom>
                      <cf __type="bin" __size="16">f34255041131eddfc769181b8f33892e</cf>
                      <qcf __type="bin" __size="32">aa4a965aa8c2c169d145e75b5da93879cd8ad1a3f32185662dc54341263dbb03</qcf>
                    </kingdom>
                    """;

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify binary data correctly converted
            Assert.Equal("f34255041131eddfc769181b8f33892e", result.Root.Element("cf").Value);
            Assert.Equal("aa4a965aa8c2c169d145e75b5da93879cd8ad1a3f32185662dc54341263dbb03", result.Root.Element("qcf").Value);

            // Verify size attribute preservation
            Assert.Equal("16", result.Root.Element("cf").Attribute("__size").Value);
            Assert.Equal("32", result.Root.Element("qcf").Attribute("__size").Value);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(formatXml, result.ToString(SaveOptions.DisableFormatting));
        }

        [Fact]
        public void TestEmptyNodes()
        {
            var xml = """
                      <kingdom>
                        <test>
                          <haha />
                        </test>
                      </kingdom>
                      """;

            // Convert to Kbin and read back
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify empty node structure preservation
            Assert.NotNull(result.Root.Element("test"));
            Assert.NotNull(result.Root.Element("test").Element("haha"));
            Assert.Equal(string.Empty, result.Root.Element("test").Element("haha").Value);

            var formatXml = XDocument.Parse(xml).ToString(SaveOptions.DisableFormatting);
            Assert.Equal(formatXml, result.ToString(SaveOptions.DisableFormatting));
        }

#if !NETFRAMEWORK // 参照项目本身错误，暂时屏蔽部分测试用例
        [Theory]
        [InlineData("""
                    <root>
                        <rarity __type="u8">13</rarity>
                    </root>
                    """)]
        [InlineData("""
                    <root>
                        <rarity __type="u8">5</rarity>
                    </root>
                    """)]
        //[InlineData("""
        //            <root>
        //                <rarity __type="u8">5</rarity>
        //                <generator_no __type="u16">1</generator_no>
        //                <distribution_date __type="u32">12356134</distribution_date>
        //                <is_default __type="u64">21512976124353</is_default>
        //                <sort_no __type="s8">-121</sort_no>
        //                <genre __type="s16">-5126</genre>
        //                <limited __type="s32">-35721234</limited>
        //                <wtf_is_this __type="s64">-253178167252134</wtf_is_this>
        //            </root>
        //            """)]
        public void TestNumbers(string value)
        {
            DoWorks(value);
        }

        [Theory]
        [InlineData("""
                    <music_list>
                        <flag __type="s32" __count="16" sheet_type="0">21 52 11 53 43 134 21 -43 -12 -61 -13 -52 -47 -114 21 52 11 53 43 134 21 -43 -12 -61 -13 -52 -47 -114 134 21 -43 -12</flag>
                    </music_list>
                    """)]
        public void TestArrayNotValid(string value)
        {
            DoWorks(value);
        }

        [Theory]
        [InlineData("""
                    <music_list>
                        <flag __type="s32" __count="32" sheet_type="0">-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1</flag>
                        <flag __type="s32" __count="32" sheet_type="1">-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1</flag>
                        <flag __type="s32" __count="32" sheet_type="2">-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1</flag>
                        <flag __type="s32" __count="32" sheet_type="3">-1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1 -1</flag>
                    </music_list>
                    """)]
        public void TestArray(string value)
        {
            DoWorks(value);
        }

        [Theory]
        [InlineData("""
                    <response status="0" fault="0" dstid="218D0B63818DD551C2BE">
                        <game status="0" fault="0">
                            <param>
                                <info>
                                    <param __type="s32" __count="7">1 1 1 1 1 1 </param>
                                </info>
                            </param>
                        </game>
                    </response>
                    """)]
        public void ErrorCaseMid(string value)
        {
            DoWorks(value);
        }
#endif


        private void DoWorks(string value)
        {
            var xml = XElement.Parse(value);
            var compress = true;
            var bytes = new StableKbin.XmlWriter(xml, Encoding.UTF8, compress).Write();
            var bytes2 = KbinConverter.Write(xml, KnownEncodings.UTF8, new WriteOptions() { Compress = compress, StrictMode = false });

            var result = new StableKbin.XmlReader(bytes).ReadLinq().ToString();
            var result2 = KbinConverter.ReadXmlLinq(bytes2.AsSpan()).ToString();

            _outputHelper.WriteLine(string.Join(", ", bytes.Select((k, i) => $"{i}: 0x{k:X2}")));
            _outputHelper.WriteLine(string.Join(", ", bytes2.Select((k, i) => $"{i}: 0x{k:X2}")));

            _outputHelper.WriteLine(result);
            _outputHelper.WriteLine(result2);
            Assert.Equal(bytes, bytes2);
            Assert.Equal(result, result2);
        }

        #endregion
    }
}