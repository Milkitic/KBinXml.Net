# KbinXml.Net

[![NuGet](https://img.shields.io/nuget/v/KbinXml.Net.svg)](https://www.nuget.org/packages/KbinXml.Net/)
[![License](https://img.shields.io/badge/license-Apache%202.0-blue.svg)](LICENSE)

High performance .NET library for encoding/decoding Konami's binary XML format (kbin).

## Features

- **High Performance**: Optimized for minimal RAM usage and CPU time
- **Multi-Framework Support**: Compatible with .NET Standard 2.0, .NET 8.0, and .NET 9.0
- **Multiple XML APIs**: Support for XmlDocument, XDocument (LINQ-to-XML), and raw byte arrays
- **Encoding Support**: Full support for UTF-8, Shift-JIS, ASCII, ISO-8859-1, and EUC-JP encodings
- **Compression**: Built-in SixBit compression algorithm support
- **Memory Efficient**: Uses RecyclableMemoryStream for optimal memory management
- **Thread Safe**: All public APIs are thread-safe for concurrent usage

## Installation

Install via NuGet Package Manager:

```bash
dotnet add package KbinXml.Net
```

Or via Package Manager Console:

```powershell
Install-Package KbinXml.Net
```

## Quick Start

### Reading kbin Files

```csharp
using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net;

// Read kbin file
byte[] kbinData = File.ReadAllBytes("data.kbin");

// Convert to different XML formats
XDocument linqXml = KbinConverter.ReadXmlLinq(kbinData);
XmlDocument w3cXml = KbinConverter.ReadXml(kbinData);
byte[] xmlBytes = KbinConverter.ReadXmlBytes(kbinData);

// Access XML content
Console.WriteLine(linqXml.ToString());
```

### Writing kbin Files

```csharp
using System.Xml.Linq;
using KbinXml.Net;

// Create XML document
var xml = new XDocument(
    new XElement("root",
        new XElement("item", 
            new XAttribute("__type", "str"), 
            "Hello World"
        )
    )
);

// Convert to kbin format
byte[] kbinData = KbinConverter.Write(xml, KnownEncodings.UTF8);

// Save to file
File.WriteAllBytes("output.kbin", kbinData);
```

## API Reference

### Core Methods

#### Reading Methods

- `ReadXmlLinq(ReadOnlySpan<byte>, ReadOptions?)` - Convert kbin to XDocument
- `ReadXml(ReadOnlySpan<byte>, ReadOptions?)` - Convert kbin to XmlDocument  
- `ReadXmlBytes(ReadOnlySpan<byte>, ReadOptions?)` - Convert kbin to raw XML bytes
- `GetXmlStream(ReadOnlySpan<byte>, ReadOptions?)` - Get XML as Stream
- `IsKbinFormat(ReadOnlySpan<byte>)` - Check if data is valid kbin format

#### Writing Methods

**Byte Array Methods:**
- `Write(XDocument, KnownEncodings, WriteOptions?)` - Convert XDocument to kbin
- `Write(XmlDocument, KnownEncodings, WriteOptions?)` - Convert XmlDocument to kbin
- `Write(byte[], KnownEncodings, WriteOptions?)` - Convert XML bytes to kbin
- `Write(string, KnownEncodings, WriteOptions?)` - Convert XML string to kbin

**Stream-based Methods:**
- `Write(XDocument, Stream, KnownEncodings, WriteOptions?)` - Write XDocument to stream
- `Write(XmlDocument, Stream, KnownEncodings, WriteOptions?)` - Write XmlDocument to stream
- `Write(byte[], Stream, KnownEncodings, WriteOptions?)` - Write XML bytes to stream
- `Write(string, Stream, KnownEncodings, WriteOptions?)` - Write XML string to stream

### Configuration Options

#### ReadOptions

```csharp
var readOptions = new ReadOptions
{
    RepairedPrefix = "fix_" // Prefix for invalid XML element names (default: null)
};

// Usage example
XDocument xml = KbinConverter.ReadXmlLinq(kbinData, readOptions);
```

#### WriteOptions

```csharp
var writeOptions = new WriteOptions
{
    StrictMode = true,      // Enforce strict validation (default: true)
    Compress = true,        // Enable SixBit compression (default: true)
    RepairedPrefix = "fix_" // Prefix for invalid XML element names (default: null)
};

// Usage example
byte[] kbinData = KbinConverter.Write(xmlDoc, KnownEncodings.UTF8, writeOptions);
```

#### Supported Encodings

```csharp
public enum KnownEncodings
{
    ShiftJIS,    // Japanese Shift-JIS
    ASCII,       // 7-bit ASCII
    ISO_8859_1,  // Latin-1
    EUC_JP,      // Japanese EUC-JP
    UTF8         // UTF-8 (recommended)
}
```

## Advanced Usage

### Stream-based Processing

**Reading from streams:**
```csharp
using var fileStream = File.OpenRead("large-file.kbin");
using var xmlStream = KbinConverter.GetXmlStream(fileStream);
using var reader = XmlReader.Create(xmlStream);

while (reader.Read())
{
    // Process XML nodes incrementally
    if (reader.NodeType == XmlNodeType.Element)
    {
        Console.WriteLine($"Element: {reader.Name}");
    }
}
```

**Writing to streams:**
```csharp
var xml = new XDocument(
    new XElement("root",
        new XElement("item", "data")
    )
);

using var outputStream = File.Create("output.kbin");
int bytesWritten = KbinConverter.Write(xml, outputStream, KnownEncodings.UTF8);
Console.WriteLine($"Written {bytesWritten} bytes to stream");
```

### Element Name Repair

KbinXml.Net automatically handles invalid XML element names that may exist in kbin data:

```csharp
// kbin data might contain elements like "1st_item" which are invalid in XML
byte[] kbinData = File.ReadAllBytes("data.kbin");

// Reading with custom repair prefix
var readOptions = new ReadOptions { RepairedPrefix = "item_" };
XDocument xml = KbinConverter.ReadXmlLinq(kbinData, readOptions);
// Invalid "1st_item" becomes "item_1st_item" in XML

// Writing back with the same prefix maintains round-trip compatibility
var writeOptions = new WriteOptions { RepairedPrefix = "item_" };
byte[] restoredKbin = KbinConverter.Write(xml, KnownEncodings.UTF8, writeOptions);
// "item_1st_item" becomes "1st_item" again in kbin
```

### Error Handling
```csharp
try
{
    var xml = KbinConverter.ReadXmlLinq(kbinData);
}
catch (KbinException ex)
{
    Console.WriteLine($"Kbin format error: {ex.Message}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Invalid input: {ex.Message}");
}
```

### Custom Encoding with Compression

```csharp
var options = new WriteOptions
{
    Compress = true,        // Enable compression
    StrictMode = false      // Allow flexible validation
};

byte[] compressedKbin = KbinConverter.Write(xmlDoc, KnownEncodings.ShiftJIS, options);
```

## Performance

This library is engineered for **extreme performance** using cutting-edge .NET optimization techniques:

### Core Performance Features
- **🚀 Zero-Allocation Design**: Extensive use of `ref struct`, `Span<T>`, and `stackalloc` eliminates GC pressure
- **⚡ Unsafe Optimizations**: Performance-critical Sixbit encoding uses `unsafe` code with direct pointer operations
- **🔄 Memory Pooling**: `RecyclableMemoryStream` and `ArrayPool<T>` minimize heap allocations

### Advanced Optimizations
- **Layered Architecture**: Decoupled `Providers` (XDocument/XmlWriter) and `TypeConverters` for optimal performance paths
- **Platform-Specific Code**: Conditional compilation leverages .NET Core 3.1+ optimizations vs .NET Framework
- **Batch Processing**: Sixbit encoding processes 4 chunks simultaneously with loop unrolling

### Benchmark-Driven Development
All optimizations are validated through comprehensive benchmarking using **BenchmarkDotNet**. Performance comparison results against alternative implementations are available in the [benchmark-results](benchmark-results/) directory.

## Requirements

- **.NET Standard 2.0** or higher
- **Supported Runtimes**:
  - .NET Framework 4.6.1+
  - .NET Core 2.0+
  - .NET 5.0+
  - .NET 8.0+
  - .NET 9.0+

## License

This project is licensed under the **Apache License 2.0** - see the [LICENSE](LICENSE) file for details.

---

**Note**: This library is for educational and interoperability purposes. Please respect Konami's intellectual property rights when using this software.
