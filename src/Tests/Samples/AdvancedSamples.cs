using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net;

namespace Samples;

/// <summary>
/// Advanced usage examples for KbinXml.Net
/// </summary>
public static class AdvancedSamples
{
    /// <summary>
    /// Demonstrates stream-based processing for large files
    /// </summary>
    public static void StreamProcessingExample()
    {
        Console.WriteLine("=== Stream Processing Example ===");

        // Create a larger XML document for demonstration
        var largeXml = new XDocument(
            new XElement("root",
                new XElement("metadata",
                    new XElement("title", new XAttribute("__type", "str"),
                        "Large Dataset"),
                    new XElement("recordCount", new XAttribute("__type", "s32"),
                        1000)
                )
            )
        );

        // Add many records to simulate a large file
        var dataElement = new XElement("data");
        for (int i = 1; i <= 100; i++)
        {
            dataElement.Add(new XElement("record",
                new XAttribute("id", i),
                new XElement("name", new XAttribute("__type", "str"),
                    $"Record {i}"),
                new XElement("value", new XAttribute("__type", "s32"),
                    i * 10),
                new XElement("timestamp", new XAttribute("__type", "str"),
                    DateTime.Now.AddMinutes(i).ToString("yyyy-MM-dd HH:mm:ss"))
            ));
        }

        largeXml.Root!.Add(dataElement);

        // Convert to KBin and save
        byte[] kbinData = KbinConverter.Write(largeXml, KnownEncodings.UTF8);
        File.WriteAllBytes("large-dataset.kbin", kbinData);
        Console.WriteLine($"Created large KBin file: {kbinData.Length} bytes");

        // Stream-based reading - memory efficient for large files
        Console.WriteLine("\nReading with stream-based approach:");
        byte[] fileData = File.ReadAllBytes("large-dataset.kbin");
        using var xmlStream = KbinConverter.GetXmlStream(fileData);
        using var reader = XmlReader.Create(xmlStream);

        int elementCount = 0;
        while (reader.Read())
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                elementCount++;
                if (reader.Name == "record")
                {
                    string? id = reader.GetAttribute("id");
                    Console.WriteLine($"Processing record ID: {id}");

                    // Only show first 5 records to avoid spam
                    if (elementCount > 10) break;
                }
            }
        }

        Console.WriteLine($"Processed {elementCount} elements using streaming");
    }

    /// <summary>
    /// Demonstrates different encoding options
    /// </summary>
    public static void EncodingExample()
    {
        Console.WriteLine("\n=== Encoding Example ===");

        var xmlWithUnicode = new XDocument(
            new XElement("root",
                new XElement("japanese", new XAttribute("__type", "str"),
                    "こんにちは"),
                new XElement("chinese", new XAttribute("__type", "str"),
                    "你好"),
                new XElement("korean", new XAttribute("__type", "str"),
                    "안녕하세요"),
                new XElement("english", new XAttribute("__type", "str"),
                    "Hello")
            )
        );

        // Test different encodings
        var encodings = new[]
        {
            KnownEncodings.UTF8,
            KnownEncodings.ShiftJIS,
            KnownEncodings.EUC_JP
        };

        foreach (var encoding in encodings)
        {
            try
            {
                byte[] encoded = KbinConverter.Write(xmlWithUnicode, encoding);
                XDocument decoded = KbinConverter.ReadXmlLinq(encoded);

                Console.WriteLine($"{encoding}: {encoded.Length} bytes");
                Console.WriteLine($"  Japanese text: {decoded.Root?.Element("japanese")?.Value}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{encoding}: Error - {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Demonstrates compression effects
    /// </summary>
    public static void CompressionExample()
    {
        Console.WriteLine("\n=== Compression Example ===");

        // Create XML with repetitive data (good for compression)
        var repetitiveXml = new XDocument(
            new XElement("root")
        );

        var root = repetitiveXml.Root!;
        for (int i = 0; i < 50; i++)
        {
            root.Add(new XElement("item",
                new XAttribute("type", "standard"),
                new XElement("category", new XAttribute("__type", "str"),
                    "electronics"),
                new XElement("brand", new XAttribute("__type", "str"),
                    "SampleBrand"),
                new XElement("model", new XAttribute("__type", "str"),
                    $"Model-{i:D3}"),
                new XElement("description", new XAttribute("__type", "str"),
                    "This is a sample product description that repeats for demonstration purposes.")
            ));
        }

        // Compare with and without compression
        var compressedOptions = new WriteOptions { Compress = true };
        var uncompressedOptions = new WriteOptions { Compress = false };

        byte[] compressed = KbinConverter.Write(repetitiveXml, KnownEncodings.UTF8, compressedOptions);
        byte[] uncompressed = KbinConverter.Write(repetitiveXml, KnownEncodings.UTF8, uncompressedOptions);

        Console.WriteLine($"Original XML size: {repetitiveXml.ToString().Length} characters");
        Console.WriteLine($"Compressed KBin: {compressed.Length} bytes");
        Console.WriteLine($"Uncompressed KBin: {uncompressed.Length} bytes");
        Console.WriteLine($"Compression ratio: {(double)compressed.Length / uncompressed.Length:P1}");

        // Verify both can be read back correctly
        var decompressed1 = KbinConverter.ReadXmlLinq(compressed);
        var decompressed2 = KbinConverter.ReadXmlLinq(uncompressed);

        bool identical = decompressed1.ToString() == decompressed2.ToString();
        Console.WriteLine($"Decompressed data identical: {identical}");
    }

    /// <summary>
    /// Demonstrates performance considerations
    /// </summary>
    public static void PerformanceExample()
    {
        Console.WriteLine("\n=== Performance Example ===");

        var testXml = new XDocument(
            new XElement("root",
                new XElement("data", new XAttribute("__type", "str"),
                    "Sample content for performance testing")
            )
        );

        // Warm up
        for (int i = 0; i < 10; i++)
        {
            var data = KbinConverter.Write(testXml, KnownEncodings.UTF8);
            KbinConverter.ReadXmlLinq(data);
        }

        // Measure performance
        var sw = System.Diagnostics.Stopwatch.StartNew();
        const int iterations = 1000;

        for (int i = 0; i < iterations; i++)
        {
            byte[] kbinData = KbinConverter.Write(testXml, KnownEncodings.UTF8);
            XDocument result = KbinConverter.ReadXmlLinq(kbinData);
        }

        sw.Stop();
        Console.WriteLine($"Completed {iterations} round-trip conversions in {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"Average time per conversion: {(double)sw.ElapsedMilliseconds / iterations:F2}ms");
    }

    /// <summary>
    /// Demonstrates working with different XML APIs
    /// </summary>
    public static void XmlApiExample()
    {
        Console.WriteLine("\n=== XML API Example ===");

        var sampleXml = new XDocument(
            new XElement("root",
                new XElement("item", new XAttribute("__type", "str"),
                    "test data")
            )
        );

        byte[] kbinData = KbinConverter.Write(sampleXml, KnownEncodings.UTF8);

        // Read as LINQ to XML (XDocument)
        XDocument linqXml = KbinConverter.ReadXmlLinq(kbinData);
        Console.WriteLine("LINQ to XML approach:");
        Console.WriteLine($"  Root element: {linqXml.Root?.Name}");
        Console.WriteLine($"  Item value: {linqXml.Root?.Element("item")?.Value}");

        // Read as W3C DOM (XmlDocument)
        XmlDocument domXml = KbinConverter.ReadXml(kbinData);
        Console.WriteLine("\nW3C DOM approach:");
        Console.WriteLine($"  Root element: {domXml.DocumentElement?.Name}");
        Console.WriteLine($"  Item value: {domXml.DocumentElement?["item"]?.InnerText}");

        // Read as raw XML bytes
        byte[] xmlBytes = KbinConverter.ReadXmlBytes(kbinData);
        string xmlString = System.Text.Encoding.UTF8.GetString(xmlBytes);
        Console.WriteLine("\nRaw XML bytes approach:");
        Console.WriteLine($"  XML length: {xmlBytes.Length} bytes");
        Console.WriteLine($"  XML preview: {xmlString.Substring(0, Math.Min(100, xmlString.Length))}...");
    }
}