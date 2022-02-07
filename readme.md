# KBinXml.Net

High performance .NET library for encoding/decoding Komani's binary XML format.

## Usage

```cs
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net;

namespace ManualTests;

public class Program
{
    static void Main(string[] args)
    {
        byte[] kbin = File.ReadAllBytes("test.bin");

        byte[] xmlBytes = KbinConverter.ReadXmlBytes(kbin);
        XDocument linq = KbinConverter.ReadXmlLinq(kbin);
        XmlDocument w3cXml = KbinConverter.ReadXml(kbin);

        string xmlStr = linq.ToString();

        byte[] newKbin1 = KbinConverter.Write(xmlBytes, KnownEncodings.UTF8);
        byte[] newKbin2 = KbinConverter.Write(linq, KnownEncodings.UTF8);
        byte[] newKbin3 = KbinConverter.Write(xmlStr, KnownEncodings.UTF8);

        Debug.Assert(newKbin1.SequenceEqual(newKbin2));
        Debug.Assert(newKbin2.SequenceEqual(newKbin3));

        Console.WriteLine(xmlStr);
    }
}
```

## Performance

This library is inspired by [FSH-B/kbinxmlcs](https://github.com/FSH-B/kbinxmlcs) and optimized for taking RAM usage (including allocating and GC) and CPU time as less as possible. The library was both optimized for latest .NET Core (3.1+) and .NET Framework (4.6.1+), which means even on the legacy .NET Framework, the library will still have appreciable performance.
