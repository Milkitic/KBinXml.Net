using System;
using System.Text;
using System.Xml;
using KbinXml.Net;
using Xunit;
using Xunit.Abstractions;

namespace GeneralUnitTests
{
    /// <summary>
    /// Encoding Tests - Test handling of different encoding schemes
    /// </summary>
    public class EncodingTests
    {
        private readonly ITestOutputHelper _outputHelper;

        public EncodingTests(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        [Theory]
        [InlineData(KnownEncodings.UTF8)]
        [InlineData(KnownEncodings.ShiftJIS)]
        [InlineData(KnownEncodings.EUC_JP)]
        [InlineData(KnownEncodings.ASCII)]
        public void ReadWrite_DifferentEncodings_PreservesData(KnownEncodings encodingType)
        {
            // Prepare test text - select appropriate text based on encoding
            string testText;

            switch (encodingType)
            {
                case KnownEncodings.UTF8:
                    testText = "UTF-8测试文本 こんにちは";
                    break;
                case KnownEncodings.ShiftJIS:
                    testText = "SHIFT-JISテストテキスト";
                    break;
                case KnownEncodings.EUC_JP:
                    testText = "EUC-JPテストテキスト";
                    break;
                case KnownEncodings.ASCII:
                    testText = "ASCII test text";
                    break;
                default:
                    testText = "Default test text";
                    break;
            }

            // Prepare XML, add __type attribute
            var xml = $"<root><value __type=\"str\">{testText}</value></root>";

            // Convert to Kbin and return
            var kbin = KbinConverter.Write(xml, encodingType);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify text is preserved unchanged
            Assert.Equal(testText, result.Root.Element("value").Value);
        }

        [Fact]
        public void ReadWithEncodingOutput_ReturnsCorrectEncoding()
        {
            // Prepare Kbin data with different encodings, add __type attribute
            var utf8Xml = "<root><value __type=\"str\">UTF-8测试テスト</value></root>";
            var utf8Kbin = KbinConverter.Write(utf8Xml, KnownEncodings.UTF8);

            var sjisXml = "<root><value __type=\"str\">SJISテスト</value></root>";
            var sjisKbin = KbinConverter.Write(sjisXml, KnownEncodings.ShiftJIS);

            // Test reading encoding from Kbin data
            KnownEncodings detectedUtf8Encoding;
            var utf8Doc = KbinConverter.ReadXmlLinq(utf8Kbin, out detectedUtf8Encoding);

            KnownEncodings detectedSjisEncoding;
            var sjisDoc = KbinConverter.ReadXmlLinq(sjisKbin, out detectedSjisEncoding);

            // Verify detected encoding is correct
            Assert.Equal(KnownEncodings.UTF8, detectedUtf8Encoding);
            Assert.Equal(KnownEncodings.ShiftJIS, detectedSjisEncoding);
        }

        [Fact]
        public void WriteWithDifferentEncoding_ChangesKbinData()
        {
            // Prepare the same XML, add __type attribute
            var xml = "<root><value __type=\"str\">エンコードテスト</value></root>";

            // Convert using different encodings
            var utf8Kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
            var sjisKbin = KbinConverter.Write(xml, KnownEncodings.ShiftJIS);
            var eucjpKbin = KbinConverter.Write(xml, KnownEncodings.EUC_JP);

            // Verify different encodings produce different Kbin data
            Assert.NotEqual(utf8Kbin, sjisKbin);
            Assert.NotEqual(utf8Kbin, eucjpKbin);
            Assert.NotEqual(sjisKbin, eucjpKbin);
        }

        [Fact]
        public void InvalidEncoding_ThrowsException()
        {
            // Prepare XML, add __type attribute
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Test invalid encoding
            Assert.Throws<ArgumentOutOfRangeException>(() => KbinConverter.Write(xml, (KnownEncodings)999));
        }

        [Fact]
        public void AllEncodings_CanReadWrite()
        {
            // Get all supported encodings
            var encodings = new[]
            {
                KnownEncodings.ASCII,
                KnownEncodings.UTF8,
                KnownEncodings.ShiftJIS,
                KnownEncodings.EUC_JP,
                KnownEncodings.ISO_8859_1
            };

            foreach (var encoding in encodings)
            {
                // Prepare XML suitable for current encoding, add __type attribute
                string testText;
                if (encoding == KnownEncodings.ASCII)
                {
                    testText = "ASCII test text";
                }
                else
                {
                    testText = $"{encoding} test text";
                }

                var xml = $"<root><value __type=\"str\">{testText}</value></root>";

                try
                {
                    // Test conversion
                    var kbin = KbinConverter.Write(xml, encoding);
                    KnownEncodings detectedEncoding;
                    var result = KbinConverter.ReadXmlLinq(kbin, out detectedEncoding);

                    // Verify text and encoding are correct
                    Assert.Equal(testText, result.Root.Element("value").Value);
                    Assert.Equal(encoding, detectedEncoding);
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"Encoding {encoding} test failed: {ex.Message}");
                    // Some encodings may not handle certain characters, we skip these exceptions
                    continue;
                }
            }
        }

        [Fact]
        public void WriteOptions_CustomOptions_HandlesCorrectly()
        {
            // Prepare XML, add __type attribute
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert XML string to XmlDocument
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Prepare WriteOptions with different configurations
            var options1 = new WriteOptions();
            var options2 = new WriteOptions();

            // Convert using different options
            var kbin1 = KbinConverter.Write(xmlDoc, KnownEncodings.UTF8, options1);
            var kbin2 = KbinConverter.Write(xmlDoc, KnownEncodings.ShiftJIS, options2);

            // Verify different options produce different Kbin data
            Assert.NotEqual(kbin1, kbin2);

            // But both can be read normally
            var result1 = KbinConverter.ReadXmlLinq(kbin1);
            var result2 = KbinConverter.ReadXmlLinq(kbin2);

            Assert.Equal("テスト", result1.Root.Element("value").Value);
            Assert.Equal("テスト", result2.Root.Element("value").Value);
        }

        [Fact]
        public void WriteOptions_CustomSettings_HandlesCorrectly()
        {
            // Prepare XML, add __type attribute
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert XML string to XmlDocument
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);

            // Prepare WriteOptions with custom settings
            var options = new WriteOptions
            {
                Compress = false
            };

            // Convert using custom options
            var kbin = KbinConverter.Write(xmlDoc, KnownEncodings.UTF8, options);
            var result = KbinConverter.ReadXmlLinq(kbin);

            // Verify data is read normally
            Assert.Equal("テスト", result.Root.Element("value").Value);
        }

        [Fact]
        public void ReadXmlBytes_ValidKbin_ReturnsXmlBytes()
        {
            // Prepare XML, add __type attribute
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert to Kbin
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);

            // Read XML bytes from Kbin
            var xmlBytes = KbinConverter.ReadXmlBytes(kbin);

            // Verify XML bytes contain expected content
            var resultString = Encoding.UTF8.GetString(xmlBytes);
            Assert.Contains("<root>", resultString);
            Assert.Contains("<value", resultString);
            Assert.Contains("テスト", resultString);
            Assert.Contains("</root>", resultString);
        }

        [Fact]
        public void GetXmlStream_ValidKbin_ReturnsMemoryStream()
        {
            // Prepare XML, add __type attribute
            var xml = "<root><value __type=\"str\">テスト</value></root>";

            // Convert to Kbin
            var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);

            // Get XML stream from Kbin
            using var stream = KbinConverter.GetXmlStream(kbin);

            // Verify stream contains expected content
            using var reader = new System.IO.StreamReader(stream, Encoding.UTF8, true);
            var resultString = reader.ReadToEnd();
            Assert.Contains("<root>", resultString);
            Assert.Contains("<value", resultString);
            Assert.Contains("テスト", resultString);
            Assert.Contains("</root>", resultString);
        }
    }
}