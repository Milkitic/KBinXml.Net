using System;
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
    /// Converts KBin binary data to an <see cref="XDocument"/> representation.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>An <see cref="XDocument"/> containing the parsed XML structure.</returns>
    /// <exception cref="KbinException">
    /// Thrown when invalid KBin data is detected (invalid signature, encoding mismatch, or unknown node types).
    /// </exception>
    /// <remarks>
    /// <para>This method uses LINQ-to-XML for document construction.</para>
    /// <para>If <paramref name="readOptions"/> is null, default read options will be used.</para>
    /// </remarks>
    public static XDocument ReadXmlLinq(Memory<byte> sourceBuffer, ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var xDocument = (XDocument)ReaderImpl(sourceBuffer, e => new XDocumentProvider(e, readOptions),
            out var knownEncoding);
        return xDocument;
    }

    /// <summary>
    /// Converts KBin binary data to an <see cref="XDocument"/> and outputs the detected encoding.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="knownEncodings">When this method returns, contains the detected encoding used in the KBin data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>An <see cref="XDocument"/> containing the parsed XML structure.</returns>
    /// <inheritdoc cref="ReadXmlLinq(Memory{byte}, ReadOptions?)"/>
    public static XDocument ReadXmlLinq(Memory<byte> sourceBuffer, out KnownEncodings knownEncodings,
        ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var xDocument =
            (XDocument)ReaderImpl(sourceBuffer, e => new XDocumentProvider(e, readOptions), out knownEncodings);
        return xDocument;
    }
    
    /// <summary>
    /// Converts KBin binary data to raw XML bytes.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>A byte array containing the XML document in UTF-8 encoding.</returns>
    /// <inheritdoc cref="ReadXmlLinq(Memory{byte}, ReadOptions?)"/>
    /// <remarks>
    /// The resulting byte array contains standard XML 1.0 formatted data without
    /// Byte Order Mark (BOM) by default.
    /// </remarks>
    public static byte[] ReadXmlBytes(Memory<byte> sourceBuffer, ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var bytes = (byte[])ReaderImpl(sourceBuffer, e => new XmlWriterProvider(e, readOptions), out var knownEncoding);
        return bytes;
    }
    
    /// <summary>
    /// Converts KBin binary data to raw XML bytes and outputs the detected encoding.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="knownEncodings">When this method returns, contains the detected encoding used in the KBin data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>A byte array containing the XML document in UTF-8 encoding.</returns>
    /// <inheritdoc cref="ReadXmlBytes(Memory{byte}, ReadOptions?)"/>
    public static byte[] ReadXmlBytes(Memory<byte> sourceBuffer, out KnownEncodings knownEncodings,
        ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var bytes = (byte[])ReaderImpl(sourceBuffer, e => new XmlWriterProvider(e, readOptions), out knownEncodings);
        return bytes;
    }
    
    /// <summary>
    /// Converts KBin binary data to an <see cref="XmlDocument"/> representation.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>An <see cref="XmlDocument"/> containing the parsed XML structure.</returns>
    /// <inheritdoc cref="ReadXmlLinq(Memory{byte}, ReadOptions?)"/>
    /// <remarks>
    /// This method uses the classic <see cref="XmlDocument"/> API which implements
    /// the W3C Document Object Model (DOM) Level 1 Core specification.
    /// </remarks>
    public static XmlDocument ReadXml(Memory<byte> sourceBuffer, ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var xmlDocument = (XmlDocument)ReaderImpl(sourceBuffer, e => new XmlDocumentProvider(e, readOptions),
            out var knownEncoding);
        return xmlDocument;
    }
    
    /// <summary>
    /// Converts KBin binary data to an <see cref="XmlDocument"/> and outputs the detected encoding.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="knownEncodings">When this method returns, contains the detected encoding used in the KBin data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>An <see cref="XmlDocument"/> containing the parsed XML structure.</returns>
    /// <inheritdoc cref="ReadXml(Memory{byte}, ReadOptions?)"/>
    public static XmlDocument ReadXml(Memory<byte> sourceBuffer, out KnownEncodings knownEncodings,
        ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var xmlDocument = (XmlDocument)ReaderImpl(sourceBuffer, e => new XmlDocumentProvider(e, readOptions),
            out knownEncodings);
        return xmlDocument;
    }

    private static object ReaderImpl(Memory<byte> sourceBuffer, Func<Encoding, WriterProvider> createWriterProvider,
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
                Logger.LogNodeControl(nodeType, pos, array);

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
                        Logger.LogStructElement(elementName, pos);
                        writerProvider.WriteStartElement(elementName);
                        break;
                    case ControlType.Attribute:
                        var attr = nodeReader.ReadString(out pos);
                        Logger.LogAttributeName(attr, pos);
                        var strLen = dataReader.ReadS32(out pos, out var flag);
                        Logger.LogAttributeLength(strLen, pos, flag);
                        var value = dataReader.ReadString(strLen, out pos, out flag);
                        Logger.LogAttributeValue(value, pos, flag);
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
                Logger.LogNodeData(propertyType, pos, array);

                if (holdValue != null)
                {
                    writerProvider.WriteElementValue(holdValue);
                    holdValue = null;
                }

                var elementName = nodeReader.ReadString(out pos);
                Logger.LogDataElement(elementName, pos);
                writerProvider.WriteStartElement(elementName);

                writerProvider.WriteStartAttribute("__type");
                writerProvider.WriteAttributeValue(propertyType.Name);
                writerProvider.WriteEndAttribute();

                currentType = propertyType.Name;

                int arraySize;
                if (array || propertyType.Name is "str" or "bin")
                {
                    arraySize = dataReader.ReadS32(out pos, out var flag); // Total size.
                    Logger.LogArraySize(arraySize, pos, flag);
                }
                else
                {
                    arraySize = propertyType.Size * propertyType.Count;
                }

                if (propertyType.Name == "str")
                {
                    holdValue = dataReader.ReadString(arraySize, out pos, out var flag);
                    Logger.LogStringValue(holdValue, pos, flag);
                }
                else if (propertyType.Name == "bin")
                {
                    writerProvider.WriteStartAttribute("__size");
                    writerProvider.WriteAttributeValue(arraySize.ToString());
                    writerProvider.WriteEndAttribute();
                    holdValue = dataReader.ReadBinary(arraySize, out pos, out var flag);
                    Logger.LogBinaryValue(holdValue, pos, flag);
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
                    Logger.LogArrayValue(holdValue, pos, flag);
                }
            }
            else
            {
                throw new KbinException($"Unknown node type: {nodeType}");
            }
        }
    }

    [InlineMethod.Inline]
    private static ReadContext GetReadContext(Memory<byte> sourceBuffer,
        Func<Encoding, WriterProvider> createWriterProvider)
    {
        //Read header section.
        int pos;
        var binaryBuffer = new BeBinaryReader(sourceBuffer);
        var signature = binaryBuffer.ReadU8(out pos, out _);
        Logger.LogSignature(signature, pos);

        var compressionFlag = binaryBuffer.ReadU8(out pos, out _);
        Logger.LogCompression(signature, pos);

        var encodingFlag = binaryBuffer.ReadU8(out pos, out _);
        Logger.LogEncoding(encodingFlag, pos);
        var encodingFlagNot = binaryBuffer.ReadU8(out pos, out _);
        Logger.LogEncodingNot(encodingFlagNot, pos);

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
        Logger.LogNodeLength(nodeLength, pos);
        var nodeReader = new NodeReader(sourceBuffer.Slice(8, nodeLength), 8, compressed, encoding);

        var dataLength = BitConverterHelper.ToBeInt32(sourceBuffer.Slice(nodeLength + 8, 4).Span);
        Logger.LogDataLength(nodeLength, pos);
        var dataReader = new DataReader(sourceBuffer.Slice(nodeLength + 12, dataLength), nodeLength + 12, encoding);

        var readProvider = createWriterProvider(encoding);

        var readContext = new ReadContext(nodeReader, dataReader, readProvider, encoding.ToKnownEncoding());
        return readContext;
    }

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