using System;
using System.Linq;
using System.Text;
using KbinXml.Net;
using Xunit;
using Xunit.Abstractions;

namespace GeneralUnitTests
{
    /// <summary>
    /// Debug Unit Tests, used to locate encoding issues
    /// </summary>
    public class DebugUnitTest
    {
        private readonly ITestOutputHelper _outputHelper;

        public DebugUnitTest(ITestOutputHelper outputHelper)
        {
            _outputHelper = outputHelper;
#if NETCOREAPP3_1_OR_GREATER
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
#endif
        }

        [Fact]
        public void Debug_EncodingProcess()
        {
            // Prepare a simple XML, add __type attribute
            var testXml = "<root><value __type=\"str\">テスト</value></root>";
            _outputHelper.WriteLine($"Original XML: {testXml}");

            // Generate Kbin binary data
            var kbin = KbinConverter.Write(testXml, KnownEncodings.UTF8);
            _outputHelper.WriteLine($"Kbin size: {kbin.Length} bytes");

            // Print the first 64 bytes of Kbin binary data for debugging
            _outputHelper.WriteLine("First 64 bytes of Kbin data:");
            for (int i = 0; i < Math.Min(64, kbin.Length); i++)
            {
                _outputHelper.WriteLine($"[{i,2}] 0x{kbin[i]:X2}");
            }

            // Try reading with different encodings
            foreach (KnownEncodings encoding in Enum.GetValues(typeof(KnownEncodings)))
            {
                try
                {
                    _outputHelper.WriteLine($"\nTrying to decode with {encoding}:");
                    var result = KbinConverter.ReadXmlLinq(kbin);
                    _outputHelper.WriteLine($"Decode result: {result}");

                    // Check value element's value
                    var valueText = result.Root?.Element("value")?.Value;
                    _outputHelper.WriteLine($"Value element value: '{valueText}'");

                    // Trace the complete XML structure
                    _outputHelper.WriteLine("XML structure:");
                    foreach (var element in result.Descendants())
                    {
                        _outputHelper.WriteLine($"Element: {element.Name}, Value: '{element.Value}', Type: {element.Attribute("__type")?.Value}");
                    }
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"Decoding exception: {ex.Message}");
                }
            }
        }

        [Fact]
        public void Debug_EncodingBuffers()
        {
            // Test different string encoding results
            var testString = "テスト";
            _outputHelper.WriteLine($"Test string: '{testString}'");

            foreach (KnownEncodings encoding in Enum.GetValues(typeof(KnownEncodings)))
            {
                try
                {
                    var encodingObj = encoding.ToEncoding();
                    var bytes = encodingObj.GetBytes(testString);
                    _outputHelper.WriteLine($"\nEncoding: {encoding}");
                    _outputHelper.WriteLine($"Encoded byte count: {bytes.Length}");
                    _outputHelper.WriteLine("Byte content:");
                    for (int i = 0; i < bytes.Length; i++)
                    {
                        _outputHelper.WriteLine($"[{i}] 0x{bytes[i]:X2}");
                    }

                    // Decode again
                    var decoded = encodingObj.GetString(bytes);
                    _outputHelper.WriteLine($"After decoding: '{decoded}'");

                    // Try using UTF8 to decode
                    var utf8Decoded = Encoding.UTF8.GetString(bytes);
                    _outputHelper.WriteLine($"UTF8 decoding: '{utf8Decoded}'");
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"Encoding exception: {ex.Message}");
                }
            }
        }

        [Fact/*(Skip = "NodeWriter doesn't support direct internal stream reading")*/]
        public void Debug_NodeWriter_WriteString()
        {
            // Since NodeWriter is an immutable ref struct, it doesn't support direct internal stream access
            // So we only test the overall XML conversion instead of testing NodeWriter separately
            var testString = "テスト";
            _outputHelper.WriteLine($"Test string: '{testString}'");

            foreach (KnownEncodings knownEncoding in Enum.GetValues(typeof(KnownEncodings)))
            {
                try
                {
                    _outputHelper.WriteLine($"\n===== Encoding: {knownEncoding} =====");

                    // Create XML with test string
                    var xml = $"<root><value __type=\"str\">{testString}</value></root>";

                    // Convert to Kbin
                    var kbin = KbinConverter.Write(xml, knownEncoding);

                    _outputHelper.WriteLine($"Kbin size: {kbin.Length} bytes");
                    _outputHelper.WriteLine("First 32 bytes of Kbin data:");
                    for (int i = 0; i < Math.Min(kbin.Length, 32); i++)
                    {
                        _outputHelper.WriteLine($"[{i,2}] 0x{kbin[i]:X2}");
                    }

                    // Convert Kbin back to XML
                    var resultXml = KbinConverter.ReadXmlLinq(kbin);
                    var valueText = resultXml.Root?.Element("value")?.Value;
                    _outputHelper.WriteLine($"Value after decoding: '{valueText}'");
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        [Fact/*(Skip = "DataReader doesn't support direct internal stream reading")*/]
        public void Debug_DataReadWrite()
        {
            // Since DataReader is an immutable ref struct, it doesn't support direct internal stream access
            // So we only test the overall XML conversion instead of testing DataReader/DataWriter separately
            var testString = "テスト";
            _outputHelper.WriteLine($"Test string: '{testString}'");

            foreach (KnownEncodings knownEncoding in Enum.GetValues(typeof(KnownEncodings)))
            {
                try
                {
                    _outputHelper.WriteLine($"\n===== Encoding: {knownEncoding} =====");

                    // Create XML with test string
                    var xml = $"<root><value __type=\"str\">{testString}</value></root>";

                    // Convert to Kbin
                    var kbin = KbinConverter.Write(xml, knownEncoding);

                    _outputHelper.WriteLine($"Kbin size: {kbin.Length} bytes");
                    _outputHelper.WriteLine("First 32 bytes of Kbin data:");
                    for (int i = 0; i < Math.Min(kbin.Length, 32); i++)
                    {
                        _outputHelper.WriteLine($"[{i,2}] 0x{kbin[i]:X2}");
                    }

                    // Convert Kbin back to XML
                    var resultXml = KbinConverter.ReadXmlLinq(kbin);
                    var valueText = resultXml.Root?.Element("value")?.Value;
                    _outputHelper.WriteLine($"Value after decoding: '{valueText}'");
                }
                catch (Exception ex)
                {
                    _outputHelper.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
                }
            }
        }

        [Fact]
        public void Debug_EncodingDetailTest()
        {
            var japaneseStrings = new[] { "テスト", "メインサーバー", "最初の項目", "コンテンツあり" };

            foreach (var testString in japaneseStrings)
            {
                _outputHelper.WriteLine($"\n==== Test string: '{testString}' ====");

                // Only use UTF8 encoding for testing, avoid using unsupported encodings
                foreach (KnownEncodings knownEncoding in new[] { KnownEncodings.UTF8, KnownEncodings.ASCII })
                {
                    try
                    {
                        var encoding = knownEncoding.ToEncoding();
                        _outputHelper.WriteLine($"\n=== Encoding: {knownEncoding} ===");

                        // 1. Encode to byte array
                        var bytes = encoding.GetBytes(testString);
                        _outputHelper.WriteLine($"Encoded byte count: {bytes.Length}");
                        _outputHelper.WriteLine("Byte content:");

                        string bytesHex = BitConverter.ToString(bytes);
                        _outputHelper.WriteLine($"HEX: {bytesHex}");

                        // 2. Generate XML and add __type attribute
                        var xml = $"<root><value __type=\"str\">{testString}</value></root>";

                        // 3. Convert to KBin
                        var kbin = KbinConverter.Write(xml, knownEncoding);
                        _outputHelper.WriteLine($"\nKbin size: {kbin.Length} bytes");

                        // 4. Convert KBin back to XML
                        var resultXml = KbinConverter.ReadXmlLinq(kbin);
                        var valueText = resultXml.Root?.Element("value")?.Value;
                        _outputHelper.WriteLine($"Value after decoding: '{valueText}'");

                        // Check if it matches
                        if (testString == valueText)
                        {
                            _outputHelper.WriteLine("✓ Match successful");
                        }
                        else
                        {
                            _outputHelper.WriteLine("✗ Match failed");
                        }
                    }
                    catch (Exception ex)
                    {
                        _outputHelper.WriteLine($"Exception: {ex.Message}");
                    }
                }
            }
        }

        [Fact]
        public void Debug_ComplexTypesTest()
        {
            _outputHelper.WriteLine("Testing read/write of various complex types");

            // Prepare XML to test various data types
            var xml = @"
            <root>
                <!-- String types -->
                <strValue __type=""str"">テスト文字列</strValue>
                
                <!-- Numerical types -->
                <intValue __type=""s32"">42</intValue>
                <floatValue __type=""float"">3.14159</floatValue>
                
                <!-- Binary types -->
                <binValue __type=""bin"" __size=""4"">DEADBEEF</binValue>
                
                <!-- Boolean types -->
                <boolValue __type=""bool"">1</boolValue>
                
                <!-- Array types -->
                <intArray __type=""s32"" __count=""3"">10 20 30</intArray>
                <strArray>
                    <item __type=""str"">最初の項目</item>
                    <item __type=""str"">2番目の項目</item>
                    <item __type=""str"">3番目の項目</item>
                </strArray>
                
                <!-- Complex structure -->
                <section id=""1"">
                    <title __type=""str"">テストセクション</title>
                    <content __type=""str"">コンテンツあり</content>
                </section>
            </root>";

            try
            {
                // Convert to Kbin
                var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
                _outputHelper.WriteLine($"Kbin size: {kbin.Length} bytes");

                // Convert Kbin back to XML
                var resultXml = KbinConverter.ReadXmlLinq(kbin);
                _outputHelper.WriteLine($"Read back XML: {resultXml}");

                // Verify each element
                _outputHelper.WriteLine("\nVerify each element value:");

                var strValue = resultXml.Root?.Element("strValue")?.Value;
                _outputHelper.WriteLine($"strValue: '{strValue}'");

                var intValue = resultXml.Root?.Element("intValue")?.Value;
                _outputHelper.WriteLine($"intValue: '{intValue}'");

                var floatValue = resultXml.Root?.Element("floatValue")?.Value;
                _outputHelper.WriteLine($"floatValue: '{floatValue}'");

                var binValue = resultXml.Root?.Element("binValue")?.Value;
                _outputHelper.WriteLine($"binValue: '{binValue}'");

                var boolValue = resultXml.Root?.Element("boolValue")?.Value;
                _outputHelper.WriteLine($"boolValue: '{boolValue}'");

                var intArray = resultXml.Root?.Element("intArray")?.Value;
                _outputHelper.WriteLine($"intArray: '{intArray}'");

                var strArrayItems = resultXml.Root?.Element("strArray")?.Elements("item").Select(e => e.Value).ToList();
                if (strArrayItems != null)
                {
                    for (int i = 0; i < strArrayItems.Count; i++)
                    {
                        _outputHelper.WriteLine($"strArray[{i}]: '{strArrayItems[i]}'");
                    }
                }

                var sectionTitle = resultXml.Root?.Element("section")?.Element("title")?.Value;
                _outputHelper.WriteLine($"section.title: '{sectionTitle}'");

                var sectionContent = resultXml.Root?.Element("section")?.Element("content")?.Value;
                _outputHelper.WriteLine($"section.content: '{sectionContent}'");
            }
            catch (Exception ex)
            {
                _outputHelper.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }

        [Fact]
        public void Debug_AllNodeTypesTest()
        {
            _outputHelper.WriteLine("Testing all supported node types");

            // Prepare XML with all supported node types
            var xml = @"
            <root>
                <!-- Basic numerical types -->
                <s8Value __type=""s8"">-128</s8Value>
                <u8Value __type=""u8"">255</u8Value>
                <s16Value __type=""s16"">-32768</s16Value>
                <u16Value __type=""u16"">65535</u16Value>
                <s32Value __type=""s32"">-2147483648</s32Value>
                <u32Value __type=""u32"">4294967295</u32Value>
                <s64Value __type=""s64"">-9223372036854775808</s64Value>
                <u64Value __type=""u64"">18446744073709551615</u64Value>
                
                <!-- String and binary -->
                <strValue __type=""str"">テスト文字列</strValue>
                <binValue __type=""bin"" __size=""4"">DEADBEEF</binValue>
                
                <!-- Special types -->
                <ipValue __type=""ip4"">192.168.1.1</ipValue>
                <timeValue __type=""time"">1633046400</timeValue>
                <floatValue __type=""float"">3.14159</floatValue>
                <doubleValue __type=""double"">1.7976931348623157E+308</doubleValue>
                
                <!-- Vector types -->
                <vec2s8 __type=""2s8"">1 -2</vec2s8>
                <vec2u8 __type=""2u8"">3 4</vec2u8>
                <vec2f __type=""2f"">1.1 2.2</vec2f>
                
                <!-- Array types -->
                <intArray __type=""s32"" __count=""3"">10 20 30</intArray>
                <boolArray __type=""bool"" __count=""4"">1 0 1 0</boolArray>
            </root>";

            try
            {
                // Convert to Kbin
                var kbin = KbinConverter.Write(xml, KnownEncodings.UTF8);
                _outputHelper.WriteLine($"Kbin size: {kbin.Length} bytes");

                // Convert Kbin back to XML
                var resultXml = KbinConverter.ReadXmlLinq(kbin);

                // Verify each element
                _outputHelper.WriteLine("\nVerify each element value:");

                foreach (var element in resultXml.Root.Elements())
                {
                    var typeName = element.Attribute("__type")?.Value;
                    _outputHelper.WriteLine($"{element.Name} ({typeName}): '{element.Value}'");
                }
            }
            catch (Exception ex)
            {
                _outputHelper.WriteLine($"Exception: {ex.Message}\n{ex.StackTrace}");
            }
        }
    }
}