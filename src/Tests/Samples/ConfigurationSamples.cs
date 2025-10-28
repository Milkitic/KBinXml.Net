using System;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net;

namespace Samples;

/// <summary>
/// Configuration and element name repair examples for KbinXml.Net
/// </summary>
public static class ConfigurationSamples
{
    /// <summary>
    /// Demonstrates ReadOptions configuration
    /// </summary>
    public static void ReadOptionsExample()
    {
        Console.WriteLine("=== ReadOptions Example ===");

        // Create sample KBin data with potentially invalid element names
        var xmlWithValidNames = new XDocument(
            new XElement("root",
                new XElement("repaired_1st_item", new XAttribute("__type", "str"),
                    "First item with repaired name"),
                new XElement("repaired_2nd_item", new XAttribute("__type", "str"),
                    "Second item with repaired name"),
                new XElement("valid_element", new XAttribute("__type", "str"),
                    "This element name was already valid"),
                new XElement("repaired_123test", new XAttribute("__type", "str"),
                    "Element starting with numbers")
            )
        );

        byte[] kbinData = KbinConverter.Write(xmlWithValidNames, KnownEncodings.UTF8);

        // Default ReadOptions (uses "repaired_" prefix)
        Console.WriteLine("Using default ReadOptions:");
        var defaultOptions = new ReadOptions();
        XDocument result1 = KbinConverter.ReadXmlLinq(kbinData, defaultOptions);
        Console.WriteLine($"Default prefix: '{defaultOptions.RepairedPrefix}'");
        Console.WriteLine("Result with default options:");
        Console.WriteLine(result1.ToString());

        // Custom ReadOptions with different prefix
        Console.WriteLine("\nUsing custom ReadOptions:");
        var customOptions = new ReadOptions { RepairedPrefix = "fixed_" };
        XDocument result2 = KbinConverter.ReadXmlLinq(kbinData, customOptions);
        Console.WriteLine($"Custom prefix: '{customOptions.RepairedPrefix}'");
        Console.WriteLine("Result with custom options:");
        Console.WriteLine(result2.ToString());

        // No prefix (empty string)
        Console.WriteLine("\nUsing empty prefix:");
        var noPrefix = new ReadOptions { RepairedPrefix = "" };
        XDocument result3 = KbinConverter.ReadXmlLinq(kbinData, noPrefix);
        Console.WriteLine("Result with no prefix:");
        Console.WriteLine(result3.ToString());
    }

    /// <summary>
    /// Demonstrates WriteOptions configuration
    /// </summary>
    public static void WriteOptionsExample()
    {
        Console.WriteLine("\n=== WriteOptions Example ===");

        var sampleXml = new XDocument(
            new XElement("root",
                new XElement("data", new XAttribute("__type", "str"),
                    "Sample content for write options testing"),
                new XElement("numbers", new XAttribute("__type", "s32"), new XAttribute("__count", "100"),
                    string.Join(" ", Enumerable.Range(1, 100))),
                new XElement("repeated", new XAttribute("__type", "str"),
                    "This text is repeated multiple times. " +
                    "This text is repeated multiple times. " +
                    "This text is repeated multiple times.")
            )
        );

        // Default WriteOptions
        Console.WriteLine("Default WriteOptions:");
        var defaultWrite = new WriteOptions();
        byte[] default_result = KbinConverter.Write(sampleXml, KnownEncodings.UTF8, defaultWrite);
        Console.WriteLine($"StrictMode: {defaultWrite.StrictMode}");
        Console.WriteLine($"Compress: {defaultWrite.Compress}");
        Console.WriteLine($"RepairedPrefix: '{defaultWrite.RepairedPrefix}'");
        Console.WriteLine($"Result size: {default_result.Length} bytes");

        // Custom WriteOptions - No compression
        Console.WriteLine("\nCustom WriteOptions (no compression):");
        var noCompress = new WriteOptions
        {
            StrictMode = true,
            Compress = false,
            RepairedPrefix = "custom_"
        };
        byte[] noCompress_result = KbinConverter.Write(sampleXml, KnownEncodings.UTF8, noCompress);
        Console.WriteLine($"StrictMode: {noCompress.StrictMode}");
        Console.WriteLine($"Compress: {noCompress.Compress}");
        Console.WriteLine($"RepairedPrefix: '{noCompress.RepairedPrefix}'");
        Console.WriteLine($"Result size: {noCompress_result.Length} bytes");

        // Custom WriteOptions - Non-strict mode
        Console.WriteLine("\nCustom WriteOptions (non-strict mode):");
        var nonStrict = new WriteOptions
        {
            StrictMode = false,
            Compress = true,
            RepairedPrefix = "lenient_"
        };
        byte[] nonStrict_result = KbinConverter.Write(sampleXml, KnownEncodings.UTF8, nonStrict);
        Console.WriteLine($"StrictMode: {nonStrict.StrictMode}");
        Console.WriteLine($"Compress: {nonStrict.Compress}");
        Console.WriteLine($"RepairedPrefix: '{nonStrict.RepairedPrefix}'");
        Console.WriteLine($"Result size: {nonStrict_result.Length} bytes");

        // Compare compression effectiveness
        double compressionRatio = (double)default_result.Length / noCompress_result.Length;
        Console.WriteLine($"\nCompression effectiveness: {compressionRatio:P1} of original size");
    }

    /// <summary>
    /// Demonstrates element name repair functionality
    /// </summary>
    public static void ElementNameRepairExample()
    {
        Console.WriteLine("\n=== Element Name Repair Example ===");

        // Simulate the repair process by creating XML with names that would be invalid
        // In real scenarios, these would come from KBin files with invalid XML names

        Console.WriteLine("Simulating KBin data with invalid XML element names...");

        // Step 1: Create XML that simulates repaired names (as if read from KBin)
        var xmlWithRepairedNames = new XDocument(
            new XElement("root",
                new XElement("fix_1st_item", new XAttribute("__type", "str"),
                    "This represents '1st_item' after repair"),
                new XElement("fix_2nd_place", new XAttribute("__type", "str"),
                    "This represents '2nd_place' after repair"),
                new XElement("fix_123abc", new XAttribute("__type", "str"),
                    "This represents '123abc' after repair"),
                new XElement("valid_name", new XAttribute("__type", "str"),
                    "This name was already valid"),
                new XElement("fix_class", new XAttribute("__type", "str"),
                    "This represents 'class' (reserved keyword) after repair")
            )
        );

        Console.WriteLine("XML with repaired element names:");
        Console.WriteLine(xmlWithRepairedNames.ToString());

        // Step 2: Write with matching RepairedPrefix to restore original names
        Console.WriteLine("\nWriting back with RepairedPrefix to restore original names...");
        var writeOptions = new WriteOptions { RepairedPrefix = "fix_" };
        byte[] kbinData = KbinConverter.Write(xmlWithRepairedNames, KnownEncodings.UTF8, writeOptions);

        // Step 3: Read back with same prefix to see the repair in action
        var readOptions = new ReadOptions { RepairedPrefix = "fix_" };
        XDocument restoredXml = KbinConverter.ReadXmlLinq(kbinData, readOptions);

        Console.WriteLine("XML after round-trip with RepairedPrefix:");
        Console.WriteLine(restoredXml.ToString());

        // Step 4: Demonstrate different prefixes
        Console.WriteLine("\nDemonstrating different repair prefixes:");

        var prefixes = new[] { "item_", "elem_", "node_", "" };

        for (var i = 0; i < prefixes.Length; i++)
        {
            var prefix = prefixes[i];
            var testReadOptions = new ReadOptions { RepairedPrefix = prefix };
            Console.WriteLine($"\nWith prefix '{prefix}':");
            XDocument testResult;
            try
            {
                testResult = KbinConverter.ReadXmlLinq(kbinData, testReadOptions);
            }
            catch (XmlException)
            {
                if (i != 3) throw;
                Console.WriteLine($"  Convert failed because of invalid node name(normal case)");
                continue;
            }

            var firstElement = testResult.Root?.Elements().FirstOrDefault();
            if (firstElement != null)
            {
                Console.WriteLine($"  First element name: '{firstElement.Name}'");
            }
        }
    }

    /// <summary>
    /// Demonstrates round-trip compatibility with element name repair
    /// </summary>
    public static void RoundTripCompatibilityExample()
    {
        Console.WriteLine("\n=== Round-Trip Compatibility Example ===");

        // Create test data with various element name scenarios
        var originalXml = new XDocument(
            new XElement("root",
                new XElement("test_normal_element", new XAttribute("__type", "str"),
                    "Normal element"),
                new XElement("repaired_1st_item", new XAttribute("__type", "str"),
                    "Simulated repaired element 1"),
                new XElement("repaired_2nd_item", new XAttribute("__type", "str"),
                    "Simulated repaired element 2"),
                new XElement("another_normal", new XAttribute("__type", "str"),
                    "Another normal element"),
                new XElement("repaired_class", new XAttribute("__type", "str"),
                    "Simulated repaired reserved word")
            )
        );

        Console.WriteLine("Original XML:");
        Console.WriteLine(originalXml.ToString());

        // Test round-trip with consistent RepairedPrefix
        var options = new WriteOptions { RepairedPrefix = "repaired_" };
        var readOptions = new ReadOptions { RepairedPrefix = "repaired_" };

        // Write to KBin
        byte[] kbinData = KbinConverter.Write(originalXml, KnownEncodings.UTF8, options);
        Console.WriteLine($"\nConverted to KBin: {kbinData.Length} bytes");

        // Read back from KBin
        XDocument roundTripXml = KbinConverter.ReadXmlLinq(kbinData, readOptions);
        Console.WriteLine("\nAfter round-trip conversion:");
        Console.WriteLine(roundTripXml.ToString());

        // Verify consistency
        bool isIdentical = originalXml.ToString() == roundTripXml.ToString();
        Console.WriteLine($"\nRound-trip successful: {isIdentical}");

        if (!isIdentical)
        {
            Console.WriteLine("Note: Differences may be due to formatting or the simulation of repair process.");
            Console.WriteLine("In real scenarios with actual invalid names, round-trip should be consistent.");
        }
    }

    /// <summary>
    /// Demonstrates validation and error scenarios with configuration
    /// </summary>
    public static void ValidationExample()
    {
        Console.WriteLine("\n=== Validation Example ===");

        var testXml = new XDocument(
            new XElement("root",
                new XElement("test", new XAttribute("__type", "str"),
                    "data")
            )
        );

        // Test strict mode vs non-strict mode
        Console.WriteLine("Testing StrictMode differences:");

        try
        {
            var strictOptions = new WriteOptions { StrictMode = true };
            byte[] strictResult = KbinConverter.Write(testXml, KnownEncodings.UTF8, strictOptions);
            Console.WriteLine($"Strict mode: Success, {strictResult.Length} bytes");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Strict mode: Error - {ex.Message}");
        }

        try
        {
            var lenientOptions = new WriteOptions { StrictMode = false };
            byte[] lenientResult = KbinConverter.Write(testXml, KnownEncodings.UTF8, lenientOptions);
            Console.WriteLine($"Lenient mode: Success, {lenientResult.Length} bytes");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Lenient mode: Error - {ex.Message}");
        }

        // Test with null options (should use defaults)
        Console.WriteLine("\nTesting with null options (uses defaults):");
        byte[] defaultResult = KbinConverter.Write(testXml, KnownEncodings.UTF8, null);
        Console.WriteLine($"Null options: Success, {defaultResult.Length} bytes");
    }
}