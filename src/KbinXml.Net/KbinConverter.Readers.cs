using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net.Internal;
using KbinXml.Net.Internal.Providers;
using KbinXml.Net.Readers;
using KbinXml.Net.Utils;

namespace KbinXml.Net;

public static partial class KbinConverter
{
    /// <summary>
    /// Reads the KBin bytes into an XML <see cref="XDocument"/>.
    /// </summary>
    /// <param name="sourceBuffer">The KBin bytes to convert.</param>
    /// <returns>Returns the <see cref="XDocument"/>.</returns>
    public static XDocument ReadXmlLinq(Memory<byte> sourceBuffer)
    {
        var xDocument = (XDocument)Read(sourceBuffer, e => new XDocumentProvider(e), out var knownEncoding);
        return xDocument;
    }

    /// <summary>
    /// Reads the KBin bytes into an XML <see cref="XDocument"/>.
    /// </summary>
    /// <param name="sourceBuffer">The KBin bytes to convert.</param>
    /// <param name="knownEncodings">The declared encoding of this KBin.</param>
    /// <returns>Returns the <see cref="XDocument"/>.</returns>
    public static XDocument ReadXmlLinq(Memory<byte> sourceBuffer, out KnownEncodings knownEncodings)
    {
        var xDocument = (XDocument)Read(sourceBuffer, e => new XDocumentProvider(e), out knownEncodings);
        return xDocument;
    }

    /// <summary>
    /// Reads the KBin bytes into an XML <see cref="T:byte[]"/>.
    /// </summary>
    /// <param name="sourceBuffer">The KBin bytes convert.</param>
    /// <returns>Returns the <see cref="T:byte[]"/>.</returns>
    public static byte[] ReadXmlBytes(Memory<byte> sourceBuffer)
    {
        var bytes = (byte[])Read(sourceBuffer, e => new XmlWriterProvider(e), out var knownEncoding);
        return bytes;
    }

    /// <summary>
    /// Reads the KBin bytes into an XML <see cref="T:byte[]"/>.
    /// </summary>
    /// <param name="sourceBuffer">The KBin bytes convert.</param>
    /// <param name="knownEncodings">The declared encoding of this KBin.</param>
    /// <returns>Returns the <see cref="T:byte[]"/>.</returns>
    public static byte[] ReadXmlBytes(Memory<byte> sourceBuffer, out KnownEncodings knownEncodings)
    {
        var bytes = (byte[])Read(sourceBuffer, e => new XmlWriterProvider(e), out knownEncodings);
        return bytes;
    }

    /// <summary>
    /// Reads the KBin bytes into an XML <see cref="XmlDocument"/>.
    /// </summary>
    /// <param name="sourceBuffer">The KBin bytes convert.</param>
    /// <returns>Returns the <see cref="XmlDocument"/>.</returns>
    public static XmlDocument ReadXml(Memory<byte> sourceBuffer)
    {
        var xmlDocument = (XmlDocument)Read(sourceBuffer, e => new XmlDocumentProvider(e), out var knownEncoding);
        return xmlDocument;
    }

    /// <summary>
    /// Reads the KBin bytes into an XML <see cref="XmlDocument"/>.
    /// </summary>
    /// <param name="sourceBuffer">The KBin bytes convert.</param>
    /// <param name="knownEncodings">The declared encoding of this KBin.</param>
    /// <returns>Returns the <see cref="XmlDocument"/>.</returns>
    public static XmlDocument ReadXml(Memory<byte> sourceBuffer, out KnownEncodings knownEncodings)
    {
        var xmlDocument = (XmlDocument)Read(sourceBuffer, e => new XmlDocumentProvider(e), out knownEncodings);
        return xmlDocument;
    }

    private static object Read(Memory<byte> sourceBuffer, Func<Encoding, WriterProvider> createWriterProvider,
        out KnownEncodings knownEncoding)
    {
        using var readContext = GetReadContext(sourceBuffer, createWriterProvider);
        knownEncoding = readContext.KnownEncoding;
        var writerProvider = readContext.WriterProvider;
        var nodeReader = readContext.NodeReader;
        var dataReader = readContext.DataReader;

        writerProvider.WriteStartDocument();
        string? currentType = null;
        string? holdValue = null;
        while (true)
        {
            var nodeType = nodeReader.ReadU8(out var pos, out _);

            //Array flag is on the second bit
            var array = (nodeType & 0x40) > 0;
            nodeType = (byte)(nodeType & ~0x40);
            if (ControlTypes.Contains(nodeType))
            {
#if DEBUG
                var str = $"node(0x{pos:X8})   NodeControlType: {(ControlType)nodeType}(0x{nodeType:X2})";
                if (array) str += ", With array flag";
                Console.WriteLine(str);
#endif

                var controlType = (ControlType)nodeType;
                switch (controlType)
                {
                    case ControlType.NodeStart:
                        if (holdValue != null)
                        {
                            writerProvider.WriteElementValue(holdValue);
                            holdValue = null;
                        }

                        var elementName = nodeReader.ReadString(out pos);
#if DEBUG
                        Console.WriteLine($"node(0x{pos:X8})   StructElement: \"{elementName}\"");
#endif
                        writerProvider.WriteStartElement(elementName);
                        break;
                    case ControlType.Attribute:
                        var attr = nodeReader.ReadString(out pos);
#if DEBUG
                        Console.WriteLine($"node(0x{pos:X8})   AttrName: \"{attr}\"");
#endif
                        var strLen = dataReader.ReadS32(out pos, out var flag);
#if DEBUG
                        Console.WriteLine($"{flag,4}(0x{pos:X8})   AttrLen: \"{strLen}\"");
#endif
                        var value = dataReader.ReadString(strLen, out pos, out flag);
#if DEBUG
                        var arr = value.SelectMany(c => char.IsControl(c) ? GetDisplayable(c) : c.ToString());
                        var str1 = new string(arr.ToArray());
                        Console.WriteLine($"{flag,4}(0x{pos:X8}) o AttrValue: \"{str1}\"");
#endif
                        // Size has been written below
                        if (currentType != "bin" || attr != "__size")
                        {
                            writerProvider.WriteStartAttribute(attr);
                            writerProvider.WriteAttributeValue(value);
                            writerProvider.WriteEndAttribute();
                        }

                        break;
                    case ControlType.NodeEnd:
                        if (holdValue != null)
                        {
                            writerProvider.WriteElementValue(holdValue);
                            holdValue = null;
                        }

                        writerProvider.WriteEndElement();
                        break;
                    case ControlType.FileEnd:
                        return writerProvider.GetResult();
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
            else if (TypeDictionary.TypeMap.TryGetValue(nodeType, out var propertyType))
            {
#if DEBUG
                var str =
                    $"node(0x{pos:X8})   NodeDataType: {propertyType.Name} (Size={propertyType.Size}, Count={propertyType.Count})";
                if (array) str += ", With array flag";
                Console.WriteLine(str);
#endif
                if (holdValue != null)
                {
                    writerProvider.WriteElementValue(holdValue);
                    holdValue = null;
                }

                var elementName = nodeReader.ReadString(out pos);
#if DEBUG
                Console.WriteLine($"node(0x{pos:X8})   DataElement: \"{elementName}\"");
#endif
                writerProvider.WriteStartElement(elementName);

                writerProvider.WriteStartAttribute("__type");
                writerProvider.WriteAttributeValue(propertyType.Name);
                writerProvider.WriteEndAttribute();

                currentType = propertyType.Name;

                int arraySize;
                if (array || propertyType.Name is "str" or "bin")
                {
                    arraySize = dataReader.ReadS32(out pos, out var flag); // Total size.
#if DEBUG
                    Console.WriteLine($"{flag,4}(0x{pos:X8})   ArraySize: {arraySize}");
#endif
                }
                else
                    arraySize = propertyType.Size * propertyType.Count;

                if (propertyType.Name == "str")
                {
                    holdValue = dataReader.ReadString(arraySize, out pos, out var flag);
#if DEBUG
                    Console.WriteLine($"{flag,4}(0x{pos:X8}) o ValString: \"{holdValue}\"");
#endif
                }
                else if (propertyType.Name == "bin")
                {
                    writerProvider.WriteStartAttribute("__size");
                    writerProvider.WriteAttributeValue(arraySize.ToString());
                    writerProvider.WriteEndAttribute();
                    holdValue = dataReader.ReadBinary(arraySize, out pos, out var flag);
#if DEBUG
                    Console.WriteLine($"{flag,4}(0x{pos:X8}) o ValBinary: \"{holdValue}\"");
#endif
                }
                else
                {
                    if (array)
                    {
                        var size = (arraySize / (propertyType.Size * propertyType.Count)).ToString();
                        writerProvider.WriteStartAttribute("__count");
                        writerProvider.WriteAttributeValue(size);
                        writerProvider.WriteEndAttribute();
                    }

                    // force to read as 32bit if is array
                    var span = array
                        ? dataReader.Read32BitAligned(arraySize, out pos, out var flag)
                        : dataReader.ReadBytes(arraySize, out pos, out flag);
                    var stringBuilder = new StringBuilder();
                    var loopCount = arraySize / propertyType.Size;
                    for (var i = 0; i < loopCount; i++)
                    {
                        var subSpan = span.Slice(i * propertyType.Size, propertyType.Size);
                        stringBuilder.Append(propertyType.GetString(subSpan.Span));
                        if (i != loopCount - 1)
                        {
#if NETCOREAPP3_1_OR_GREATER
                            stringBuilder.Append(' ');
#else
                            stringBuilder.Append(" ");
#endif
                        }
                    }

                    holdValue = stringBuilder.ToString();
#if DEBUG
                    Console.WriteLine($"{flag,4}(0x{pos:X8}) o ValArray: \"{holdValue}\"");
#endif
                }
            }
            else
            {
                throw new KbinException($"Unknown node type: {nodeType}");
            }
        }
    }

    private static ReadContext GetReadContext(Memory<byte> sourceBuffer,
        Func<Encoding, WriterProvider> createWriterProvider)
    {
        //Read header section.
        int pos;
        var binaryBuffer = new BeBinaryReader(sourceBuffer);
        var signature = binaryBuffer.ReadU8(out pos, out _);
#if DEBUG
        Console.WriteLine($"0x{pos:X8}   Signature: 0x{signature:X2}");
#endif

        var compressionFlag = binaryBuffer.ReadU8(out pos, out _);
#if DEBUG
        Console.WriteLine($"0x{pos:X8}   Compression: 0x{compressionFlag:X2}");
#endif

        var encodingFlag = binaryBuffer.ReadU8(out pos, out _);
#if DEBUG
        Console.WriteLine($"0x{pos:X8}   Encoding: 0x{encodingFlag:X2}");
#endif
        var encodingFlagNot = binaryBuffer.ReadU8(out pos, out _);
#if DEBUG
        Console.WriteLine($"0x{pos:X8}   Encoding~: 0x{encodingFlagNot:X2}");
#endif

        //Verify magic.
        if (signature != 0xA0)
            throw new KbinException($"Signature was invalid. 0x{signature:X2} != 0xA0");

        //Encoding flag should be an inverse of the fourth byte.
        if ((byte)~encodingFlag != encodingFlagNot)
            throw new KbinException(
                $"Third byte was not an inverse of the fourth. {~encodingFlag} != {encodingFlagNot}");

        var compressed = compressionFlag == 0x42;
        var encoding = EncodingDictionary.EncodingMap[encodingFlag];

        //Get buffer lengths and load.
        var nodeLength = binaryBuffer.ReadS32(out pos, out _);
#if DEBUG
        Console.WriteLine($"0x{pos:X8}   NodeLength: {nodeLength}");
#endif
        var nodeReader = new NodeReader(sourceBuffer.Slice(8, nodeLength), 8, compressed, encoding);

        var dataLength = BitConverterHelper.ToBeInt32(sourceBuffer.Slice(nodeLength + 8, 4).Span);
#if DEBUG
        Console.WriteLine($"0x{pos:X8}   DataLength: {dataLength}");
#endif
        var dataReader = new DataReader(sourceBuffer.Slice(nodeLength + 12, dataLength), nodeLength + 12, encoding);

        var readProvider = createWriterProvider(encoding);

        var readContext = new ReadContext(nodeReader, dataReader, readProvider, encoding.ToKnownEncoding());
        return readContext;
    }

#if DEBUG
    private static string GetDisplayable(char c)
    {
        if (NonPrintDict.TryGetValue(c, out var value))
        {
            return $"[\\{value}]";
        }

        return $"[\\{(int)c}]";
    }

    private static readonly Dictionary<int, string> NonPrintDict = new()
    {
        [00] = "NULL",
        [01] = "SOH",
        [02] = "STX",
        [03] = "ETX",
        [04] = "EOT",
        [05] = "ENQ",
        [06] = "ACK",
        [07] = "BEL",
        [08] = "BS",
        [09] = "HT",
        [10] = "LF",
        [11] = "VT",
        [12] = "FF",
        [13] = "CR",
        [14] = "SO",
        [15] = "SI",
        [16] = "DLE",
        [17] = "DC1",
        [18] = "DC2",
        [19] = "DC3",
        [20] = "DC4",
        [21] = "NAK",
        [22] = "SYN",
        [23] = "ETB",
        [24] = "CAN",
        [25] = "EM",
        [26] = "SUB",
        [27] = "ESC",
        [28] = "FS",
        [29] = "GS",
        [30] = "RS",
        [31] = "US",
        [127] = "DEL",
    };
#endif

    private class ReadContext : IDisposable
    {
        public ReadContext(NodeReader nodeReader, DataReader dataReader, WriterProvider writerProvider,
            KnownEncodings knownEncoding)
        {
            NodeReader = nodeReader;
            DataReader = dataReader;
            WriterProvider = writerProvider;
            KnownEncoding = knownEncoding;
        }

        public NodeReader NodeReader { get; set; }
        public DataReader DataReader { get; set; }
        public WriterProvider WriterProvider { get; set; }
        public KnownEncodings KnownEncoding { get; }

        public void Dispose()
        {
            WriterProvider.Dispose();
        }
    }
}