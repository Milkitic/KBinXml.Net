using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net;
using kbinxmlcs;

namespace ManualTests;

public class Program
{
    static void Main(string[] args)
    {
        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        SmallTest();
        InvalidTest();

        byte[] kbin = File.ReadAllBytes("data/test_case2.bin");

        byte[] xmlBytes = KbinConverter.ReadXmlBytes(kbin);
        XDocument linq = KbinConverter.ReadXmlLinq(kbin);
        XmlDocument w3cXml = KbinConverter.ReadXml(kbin);

        string xmlStr = linq.ToString();

        byte[] newKbin1 = KbinConverter.Write(xmlBytes, KnownEncodings.UTF8);
        byte[] newKbin2 = KbinConverter.Write(linq, KnownEncodings.UTF8);
        byte[] newKbin3 = KbinConverter.Write(xmlStr, KnownEncodings.UTF8);

        Debug.Assert(newKbin1.SequenceEqual(newKbin2));
        Debug.Assert(newKbin2.SequenceEqual(newKbin3));

        var kbinReader = new KbinReader(kbin);
        var linqRef = kbinReader.ReadLinq();

        var kbinWriter = new KbinWriter(linqRef, Encoding.UTF8);
        var newKbinRef = kbinWriter.Write();

        Debug.Assert(linqRef.ToString() == linq.ToString());
        Debug.Assert(newKbin2.SequenceEqual(newKbinRef));

        //Console.WriteLine(xmlStr);

        //var obj = new object();
        //int i = 0;
        //new int[10000].AsParallel().ForAll(_ =>
        //{
        //    KbinConverter.WriteRaw(str, Encoding.UTF8);
        //    lock (obj)
        //    {
        //        i++;
        //        Console.WriteLine(i);
        //    }
        //});
        //return;
    }

    private static void SmallTest()
    {
        var _kbin = KbinConverter.Write(File.ReadAllText(@"data/small.xml"), KnownEncodings.ShiftJIS);
        var linq = KbinConverter.ReadXmlLinq(_kbin);
        var _xmlStr = linq.ToString();
        KbinConverter.Write(_xmlStr, KnownEncodings.ShiftJIS, new WriteOptions { RepairedPrefix = "PREFIX_" });


        var smallText = File.ReadAllText("data/small.xml");
        byte[] smallKbin = KbinConverter.Write(smallText, KnownEncodings.ShiftJIS);
        var smallXmlRead = KbinConverter.ReadXmlBytes(smallKbin);
    }

    private static void InvalidTest()
    {
        var invalidXml = File.ReadAllText("data/konmaiquality.xml");
        byte[] kbin = KbinConverter.Write(invalidXml, KnownEncodings.ShiftJIS, new WriteOptions { RepairedPrefix = "KBIN_PREFIX_FIX_" });

        var bytesRead = KbinConverter.ReadXmlBytes(kbin, new ReadOptions { RepairedPrefix = "KBIN_PREFIX_FIX_" });
        XElement bytesReadLinq;
        using (var ms = new MemoryStream(bytesRead))
        {
            bytesReadLinq = XElement.Load(ms);
        }


        var linqRead = KbinConverter.ReadXmlLinq(kbin, new ReadOptions { RepairedPrefix = "KBIN_PREFIX_FIX_" });


        var w3cRead = KbinConverter.ReadXml(kbin, new ReadOptions { RepairedPrefix = "KBIN_PREFIX_FIX_" });
        XDocument w3cReadLinq;
        using (var nodeReader = new XmlNodeReader(w3cRead))
        {
            nodeReader.MoveToContent();
            w3cReadLinq = XDocument.Load(nodeReader);
        }
    }
}