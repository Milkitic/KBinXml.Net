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