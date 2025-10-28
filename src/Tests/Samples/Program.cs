using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net;

namespace Samples;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("KbinXml.Net Samples");
        Console.WriteLine("===================");

        try
        {
            // Run basic samples
            QuickStartSamples();
            ConfigurationSamplesDemo();
            AdvancedUsageSamples();
            ElementNameRepairSamples();
            ErrorHandlingSamples();

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("ADVANCED SAMPLES");
            Console.WriteLine(new string('=', 50));

            // Run advanced samples from separate classes
            AdvancedSamples.StreamProcessingExample();
            AdvancedSamples.EncodingExample();
            AdvancedSamples.CompressionExample();
            AdvancedSamples.PerformanceExample();
            AdvancedSamples.XmlApiExample();

            Console.WriteLine("\n" + new string('=', 50));
            Console.WriteLine("CONFIGURATION SAMPLES");
            Console.WriteLine(new string('=', 50));

            // Run configuration samples
            ConfigurationSamples.ReadOptionsExample();
            ConfigurationSamples.WriteOptionsExample();
            ConfigurationSamples.ElementNameRepairExample();
            ConfigurationSamples.RoundTripCompatibilityExample();
            ConfigurationSamples.ValidationExample();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    static void QuickStartSamples()
    {
        Console.WriteLine("\n1. Quick Start Samples");
        Console.WriteLine("----------------------");

        // Create sample XML data
        var sampleXml = new XDocument(
            new XElement("root",
                new XElement("item",
                    new XAttribute("__type", "str"),
                    new XAttribute("id", "1"),
                    "Sample data"
                ),
                new XElement("number", new XAttribute("__type", "s32"),
                    42),
                new XElement("flag", new XAttribute("__type", "bool"),
                    1)
            )
        );

        // Writing KBin data
        Console.WriteLine("Writing XML to KBin format...");
        byte[] kbinData = KbinConverter.Write(sampleXml, KnownEncodings.UTF8);
        Console.WriteLine($"Generated KBin data: {kbinData.Length} bytes");

        // Save to file for reading example
        File.WriteAllBytes("sample.kbin", kbinData);

        // Reading KBin data
        Console.WriteLine("\nReading KBin file...");
        byte[] fileData = File.ReadAllBytes("sample.kbin");

        // Convert to different XML formats
        XDocument linqXml = KbinConverter.ReadXmlLinq(fileData);
        XmlDocument w3cXml = KbinConverter.ReadXml(fileData);
        byte[] xmlBytes = KbinConverter.ReadXmlBytes(fileData);

        // Access XML content
        Console.WriteLine("LINQ XML content:");
        Console.WriteLine(linqXml.ToString());

        Console.WriteLine("\nXML as bytes length: " + xmlBytes.Length);
    }

    static void ConfigurationSamplesDemo()
    {
        Console.WriteLine("\n2. Configuration Options");
        Console.WriteLine("------------------------");

        var sampleXml = new XDocument(
            new XElement("root",
                new XElement("test", new XAttribute("__type", "str"),
                    "data")
            )
        );

        // ReadOptions example
        var readOptions = new ReadOptions
        {
            RepairedPrefix = "fix_" // Prefix for invalid XML element names (default: "repaired_")
        };

        // WriteOptions example
        var writeOptions = new WriteOptions
        {
            StrictMode = true, // Enforce strict validation (default: true)
            Compress = true, // Enable SixBit compression (default: true)
            RepairedPrefix = "fix_" // Prefix for invalid XML element names (default: "repaired_")
        };

        // Usage examples
        byte[] kbinData = KbinConverter.Write(sampleXml, KnownEncodings.UTF8, writeOptions);
        XDocument xml = KbinConverter.ReadXmlLinq(kbinData, readOptions);

        Console.WriteLine("Configuration options applied successfully");
        Console.WriteLine($"KBin data size: {kbinData.Length} bytes");

        // Supported Encodings demonstration
        Console.WriteLine("\nSupported Encodings:");
        foreach (var encoding in Enum.GetValues<KnownEncodings>())
        {
            Console.WriteLine($"- {encoding}");
        }
    }

    static void AdvancedUsageSamples()
    {
        Console.WriteLine("\n3. Advanced Usage");
        Console.WriteLine("-----------------");

        // Stream-based Processing - Reading from streams
        Console.WriteLine("Stream-based reading:");
        var sampleXml = new XDocument(
            new XElement("root",
                new XElement("item1", new XAttribute("__type", "str"),
                    "data1"),
                new XElement("item2", new XAttribute("__type", "str"),
                    "data2"),
                new XElement("item3", new XAttribute("__type", "str"),
                    "data3")
            )
        );

        // Create sample file
        byte[] kbinData = KbinConverter.Write(sampleXml, KnownEncodings.UTF8);
        File.WriteAllBytes("large-file.kbin", kbinData);

        byte[] fileData = File.ReadAllBytes("large-file.kbin");
        using var xmlStream = KbinConverter.GetXmlStream(fileData);
        using var reader = XmlReader.Create(xmlStream);

        while (reader.Read())
        {
            // Process XML nodes incrementally
            if (reader.NodeType == XmlNodeType.Element)
            {
                Console.WriteLine($"Element: {reader.Name}");
            }
        }

        // Stream-based Processing - Writing to streams
        Console.WriteLine("\nStream-based writing:");
        var xml = new XDocument(
            new XElement("root",
                new XElement("item", new XAttribute("__type", "str"),
                    "data")
            )
        );

        using var outputStream = File.Create("output.kbin");
        int bytesWritten = KbinConverter.Write(xml, outputStream, KnownEncodings.UTF8);
        Console.WriteLine($"Written {bytesWritten} bytes to stream");
    }

    static void ElementNameRepairSamples()
    {
        Console.WriteLine("\n4. Element Name Repair");
        Console.WriteLine("----------------------");

        // Simulate KBin data with invalid XML element names
        // Note: This is a demonstration - in real scenarios, such data would come from actual KBin files
        Console.WriteLine("Element Name Repair demonstration:");
        Console.WriteLine("KBin data might contain elements like '1st_item' which are invalid in XML");

        // Create XML with repaired names to simulate the repair process
        var xmlWithRepairedNames = new XDocument(
            new XElement("root",
                new XElement("item_1st_item", new XAttribute("__type", "str"),
                    "This simulates a repaired element name"),
                new XElement("item_2nd_item", new XAttribute("__type", "str"),
                    "Another repaired element"),
                new XElement("valid_element", new XAttribute("__type", "str"),
                    "This element name was already valid")
            )
        );

        // Reading with custom repair prefix
        var readOptions = new ReadOptions { RepairedPrefix = "item_" };

        // Writing back with the same prefix maintains round-trip compatibility
        var writeOptions = new WriteOptions { RepairedPrefix = "item_" };

        byte[] kbinData = KbinConverter.Write(xmlWithRepairedNames, KnownEncodings.UTF8, writeOptions);
        XDocument restoredXml = KbinConverter.ReadXmlLinq(kbinData, readOptions);

        Console.WriteLine("Original XML with repaired names:");
        Console.WriteLine(xmlWithRepairedNames.ToString());

        Console.WriteLine("\nAfter round-trip conversion:");
        Console.WriteLine(restoredXml.ToString());

        Console.WriteLine("Round-trip compatibility maintained with RepairedPrefix settings");
    }

    static void ErrorHandlingSamples()
    {
        Console.WriteLine("\n5. Error Handling");
        Console.WriteLine("-----------------");

        try
        {
            // Attempt to read invalid KBin data
            byte[] invalidData = { 0x00, 0x01, 0x02, 0x03 };
            XDocument xml = KbinConverter.ReadXmlLinq(invalidData);
        }
        catch (KbinException ex)
        {
            Console.WriteLine($"KbinException caught: {ex.Message}");
        }

        try
        {
            // Attempt to write null XML
            byte[] result = KbinConverter.Write((XDocument)null!, KnownEncodings.UTF8);
        }
        catch (KbinException ex)
        {
            Console.WriteLine($"KbinException for null input: {ex.Message}");
        }
        catch (ArgumentNullException ex)
        {
            Console.WriteLine($"ArgumentNullException: {ex.Message}");
        }

        // Validation example
        byte[] testData = File.ReadAllBytes("sample.kbin");
        bool isValidKbin = KbinConverter.IsKbinFormat(testData);
        Console.WriteLine($"Is valid KBin data: {isValidKbin}");

        Console.WriteLine("Error handling examples completed");
    }
}