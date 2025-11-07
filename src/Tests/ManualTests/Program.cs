using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net;
using KbinXml.Net.Internal.Writers;
using kbinxmlcs;
using Microsoft.IO;

namespace ManualTests;

public class Program
{
    internal static readonly RecyclableMemoryStreamManager RecyclableMemoryStreamManager = new();

    static void Main(string[] args)
    {
        //var stream = RecyclableMemoryStreamManager.GetStream(null, 204800);
        //var init = stream.GetBuffer();
        //init.AsSpan().Fill(0x80);
        //var sb = stream.GetBuffer();
        //stream.Position = 20;
        //var ok = stream.GetSpan(20);
        //for (int i = 0; i < 10; i++)
        //{
        //    ok[i] = (byte)(255 - i);
        //}
        //stream.Advance(10);
        //var g = stream.ToArray();

        //stream.Position = 10;
        //ok = stream.GetSpan(10);
        //for (int i = 0; i < 10; i++)
        //{
        //    ok[i] = (byte)(i + 1);
        //}

        //stream.Advance(10);
        //g = stream.ToArray();
        //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        if (args.Length > 0)
            DataWriterTest();
        else
            DataWriter2Test();
        //SmallTest();
        //InvalidTest();

        return;
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

    private static void DataWriterTest()
    {
        using var _stream = KbinConverter.RecyclableMemoryStreamManager.GetStream();
        _stream.Position += 100;
        var o2 = _stream.ToArray();
        for (int i = 0; i < 1000000; i++)
        {
            _stream.SetLength(0);
            using var writer = new DataWriter(Encoding.UTF8, _stream);

            writer.WriteS8(1); // 1 byte
            writer.WriteBinary("E004E0D1423A4EE2".AsSpan());
            writer.WriteS16(2); // 2 bytes
            writer.WriteS8(3); // 1 byte
            writer.WriteString("Hello".AsSpan());
            writer.WriteS16(1996); // 2 bytes
            writer.WriteS32(4); // 4 bytes
            writer.WriteU8(240); // 1 byte
            var o = writer.Stream.GetBuffer();
        }
    }

    private static void DataWriter2Test()
    {
        using var _stream = KbinConverter.RecyclableMemoryStreamManager.GetStream();
        using var writer1 = new DataWriter2(Encoding.UTF8, _stream);
        for (var i = 0; i <100; i++)
        {
            var b = 255;
            writer1.WriteS32(b);
            _stream.SetLength(_stream.Length - 4);
        }

        for (int i = 0; i < 1000000; i++)
        {
            _stream.SetLength(0);
            using var writer = new DataWriter2(Encoding.UTF8, _stream);

            writer.WriteS8(1); // 1 byte
            writer.WriteBinary("E004E0D1423A4EE2".AsSpan());
            writer.WriteS16(2); // 2 bytes
            writer.WriteS8(3); // 1 byte
            writer.WriteString("Hello".AsSpan());
            writer.WriteS16(1996); // 2 bytes
            writer.WriteS32(4); // 4 bytes
            writer.WriteU8(240); // 1 byte
            var o = writer.Stream.GetBuffer();
        }
    }

    private static void SmallTest()
    {
        var writeOptions = new WriteOptions()
        {
            Compress = true
        };
        //var smallText = File.ReadAllText("data/small.xml");
        var smallText = File.OpenRead("data/small.xml");
        for (int i = 0; i < 10000; i++)
        {
            smallText.Seek(0, SeekOrigin.Begin);
            using var stream = KbinConverter.RecyclableMemoryStreamManager.GetStream();
            //await Task.Delay(1);
            //if (i == 200)
            //{

            //}
            var length = KbinConverter.Write(smallText, stream, KnownEncodings.ShiftJIS, writeOptions);
            //var linq = KbinConverter.ReadXmlLinq(_kbin);
            //var _xmlStr = linq.ToString();
            //KbinConverter.Write(_xmlStr, KnownEncodings.ShiftJIS, new WriteOptions { RepairedPrefix = "PREFIX_" });


            //byte[] smallKbin = KbinConverter.Write(smallText, KnownEncodings.ShiftJIS);
            //var smallXmlRead = KbinConverter.ReadXmlBytes(smallKbin);
        }
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