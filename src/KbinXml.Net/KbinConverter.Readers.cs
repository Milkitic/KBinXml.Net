using System;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net.Internal;
using KbinXml.Net.Internal.Providers;
using KbinXml.Net.Internal.Readers;
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
    public static XDocument ReadXmlLinq(ReadOnlySpan<byte> sourceBuffer, ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var xDocument = (XDocument)ReaderImpl(readOptions, sourceBuffer,
            e => new XDocumentProvider(e, readOptions), out _);
        return xDocument;
    }

    /// <summary>
    /// Converts KBin binary data to an <see cref="XDocument"/> and outputs the detected encoding.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="knownEncodings">When this method returns, contains the detected encoding used in the KBin data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>An <see cref="XDocument"/> containing the parsed XML structure.</returns>
    /// <inheritdoc cref="ReadXmlLinq(ReadOnlySpan{byte}, ReadOptions?)"/>
    public static XDocument ReadXmlLinq(ReadOnlySpan<byte> sourceBuffer, out KnownEncodings knownEncodings,
        ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var xDocument =
            (XDocument)ReaderImpl(readOptions, sourceBuffer,
                e => new XDocumentProvider(e, readOptions), out knownEncodings);
        return xDocument;
    }

    /// <summary>
    /// Converts KBin binary data to raw XML bytes.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>A byte array containing the XML document in UTF-8 encoding.</returns>
    /// <inheritdoc cref="ReadXmlLinq(ReadOnlySpan{byte}, ReadOptions?)"/>
    /// <remarks>
    /// The resulting byte array contains standard XML 1.0 formatted data without
    /// Byte Order Mark (BOM) by default.
    /// </remarks>
    public static byte[] ReadXmlBytes(ReadOnlySpan<byte> sourceBuffer, ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var bytes = (byte[])ReaderImpl(readOptions, sourceBuffer,
            e => new XmlWriterProvider(e, readOptions), out _);
        return bytes;
    }

    /// <summary>
    /// Converts KBin binary data to raw XML bytes and outputs the detected encoding.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="knownEncodings">When this method returns, contains the detected encoding used in the KBin data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>A byte array containing the XML document in UTF-8 encoding.</returns>
    /// <inheritdoc cref="ReadXmlBytes(ReadOnlySpan{byte}, ReadOptions?)"/>
    public static byte[] ReadXmlBytes(ReadOnlySpan<byte> sourceBuffer, out KnownEncodings knownEncodings,
        ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var bytes = (byte[])ReaderImpl(readOptions, sourceBuffer,
            e => new XmlWriterProvider(e, readOptions), out knownEncodings);
        return bytes;
    }

    /// <summary>
    /// Converts KBin binary data to raw XML bytes.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>A MemoryStream containing the XML document in UTF-8 encoding.</returns>
    /// <inheritdoc cref="ReadXmlLinq(ReadOnlySpan{byte}, ReadOptions?)"/>
    /// <remarks>
    /// The resulting byte array contains standard XML 1.0 formatted data without
    /// Byte Order Mark (BOM) by default.
    /// </remarks>
    public static MemoryStream GetXmlStream(ReadOnlySpan<byte> sourceBuffer, ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var bytes = (MemoryStream)ReaderImpl(readOptions, sourceBuffer,
            e => new XmlWriterProvider(e, readOptions, true),
            out _);
        return bytes;
    }

    /// <summary>
    /// Converts KBin binary data to raw XML bytes and outputs the detected encoding.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="knownEncodings">When this method returns, contains the detected encoding used in the KBin data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>A MemoryStream containing the XML document in UTF-8 encoding.</returns>
    /// <inheritdoc cref="ReadXmlBytes(ReadOnlySpan{byte}, ReadOptions?)"/>
    public static MemoryStream GetXmlStream(ReadOnlySpan<byte> sourceBuffer, out KnownEncodings knownEncodings,
        ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var bytes = (MemoryStream)ReaderImpl(readOptions, sourceBuffer,
            e => new XmlWriterProvider(e, readOptions, true),
            out knownEncodings);
        return bytes;
    }

    /// <summary>
    /// Converts KBin binary data to an <see cref="XmlDocument"/> representation.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>An <see cref="XmlDocument"/> containing the parsed XML structure.</returns>
    /// <inheritdoc cref="ReadXmlLinq(ReadOnlySpan{byte}, ReadOptions?)"/>
    /// <remarks>
    /// This method uses the classic <see cref="XmlDocument"/> API which implements
    /// the W3C Document Object Model (DOM) Level 1 Core specification.
    /// </remarks>
    public static XmlDocument ReadXml(ReadOnlySpan<byte> sourceBuffer, ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var xmlDocument = (XmlDocument)ReaderImpl(readOptions, sourceBuffer,
            e => new XmlDocumentProvider(e, readOptions), out _);
        return xmlDocument;
    }

    /// <summary>
    /// Converts KBin binary data to an <see cref="XmlDocument"/> and outputs the detected encoding.
    /// </summary>
    /// <param name="sourceBuffer">The source buffer containing KBin-formatted data.</param>
    /// <param name="knownEncodings">When this method returns, contains the detected encoding used in the KBin data.</param>
    /// <param name="readOptions">Optional reading configuration options.</param>
    /// <returns>An <see cref="XmlDocument"/> containing the parsed XML structure.</returns>
    /// <inheritdoc cref="ReadXml(ReadOnlySpan{byte}, ReadOptions?)"/>
    public static XmlDocument ReadXml(ReadOnlySpan<byte> sourceBuffer, out KnownEncodings knownEncodings,
        ReadOptions? readOptions = null)
    {
        readOptions ??= new ReadOptions();
        var xmlDocument = (XmlDocument)ReaderImpl(readOptions, sourceBuffer,
            e => new XmlDocumentProvider(e, readOptions), out knownEncodings);
        return xmlDocument;
    }

    private static object ReaderImpl(ReadOptions readOptions,
        ReadOnlySpan<byte> sourceBuffer,
        Func<Encoding, WriterProvider> createWriterProvider,
        out KnownEncodings knownEncoding)
    {
        var readContext = GetReadContext(readOptions, sourceBuffer, createWriterProvider);
        try
        {
            knownEncoding = readContext.KnownEncoding;
            readContext.WriterProvider.WriteStartDocument();
            while (true)
            {
                var nodeTypeResult = readContext.NodeReader.ReadU8();
                var bNodeType = nodeTypeResult.Value;

                //Array flag is on the second bit
                var isArray = (bNodeType & 0x40) > 0;
                bNodeType = (byte)(bNodeType & ~0x40);
                if (ControlTypes.Contains(bNodeType))
                {
#if USELOG
                    Logger.LogNodeControl(bNodeType, nodeTypeResult.Value, isArray);
#endif
                    var result = readContext.ProcessControlNode(bNodeType);
                    if (result != null)
                    {
                        return result;
                    }
                }
                else if (NodeTypeFactory.TryGetNodeType(bNodeType, out var propertyType))
                {
#if USELOG
                    Logger.LogNodeData(propertyType, nodeTypeResult.Value, isArray);
#endif
                    readContext.ProcessDataNode(propertyType!, isArray);
                }
                else
                {
                    throw new KbinException($"Unknown node type: {bNodeType}");
                }
            }
        }
        finally
        {
            readContext.Dispose();
        }
    }

    private static ReadContext GetReadContext(ReadOptions readOptions,
        ReadOnlySpan<byte> sourceBuffer, Func<Encoding, WriterProvider> createWriterProvider)
    {
        //Read header section.
        var binaryBuffer = new BigEndianReader(sourceBuffer);
        var signature = binaryBuffer.ReadU8();
#if USELOG
        Logger.LogSignature(signature.Value, signature.ReadStatus.Offset);
#endif

        var compressionFlag = binaryBuffer.ReadU8();
#if USELOG
        Logger.LogCompression(compressionFlag.Value, compressionFlag.ReadStatus.Offset);
#endif

        var encodingFlag = binaryBuffer.ReadU8();
#if USELOG
        Logger.LogEncoding(encodingFlag.Value, encodingFlag.ReadStatus.Offset);
#endif
        var encodingFlagNot = binaryBuffer.ReadU8();
#if USELOG
        Logger.LogEncodingNot(encodingFlagNot.Value, encodingFlagNot.ReadStatus.Offset);
#endif

        //Verify magic.
        if (signature.Value != 0xA0)
            throw new KbinException($"Signature was invalid. 0x{signature.Value:X2} != 0xA0");

        //Encoding flag should be an inverse of the fourth byte.
        if ((byte)~encodingFlag.Value != encodingFlagNot.Value)
            throw new KbinException(
                $"Third byte was not an inverse of the fourth. {~encodingFlag.Value} != {encodingFlagNot.Value}");

        var compressed = compressionFlag.Value == 0x42;
        var encoding = EncodingDictionary.EncodingMap[encodingFlag.Value];

        //Get buffer lengths and load.
        var nodeLength = binaryBuffer.ReadS32();
#if USELOG
        Logger.LogNodeLength(nodeLength.Value, nodeLength.ReadStatus.Offset);
#endif
        var nodeReader = new NodeReader(sourceBuffer.Slice(8, nodeLength.Value), encoding, compressed);

        var dataLength = BitConverterHelper.ToBeInt32(sourceBuffer.Slice(nodeLength.Value + 8, 4));
        Logger.LogDataLength(dataLength, nodeLength.Value + 12);
        var dataReader = new DataReader(sourceBuffer.Slice(nodeLength.Value + 12, dataLength), encoding);

        var readProvider = createWriterProvider(encoding);

        var readContext = new ReadContext(readOptions, nodeReader, dataReader, readProvider, encoding.ToKnownEncoding());
        return readContext;
    }

    private ref struct ReadContext : IDisposable
    {
        public readonly WriterProvider WriterProvider;
        public readonly KnownEncodings KnownEncoding;

        private readonly ReadOptions _readOptions;

        public NodeReader NodeReader;
        public DataReader DataReader;
        public string? CurrentType;
        public string? HoldValue;

        public ReadContext(ReadOptions readOptions, NodeReader nodeReader, DataReader dataReader, WriterProvider writerProvider,
            KnownEncodings knownEncoding)
        {
            _readOptions = readOptions;
            NodeReader = nodeReader;
            DataReader = dataReader;
            WriterProvider = writerProvider;
            KnownEncoding = knownEncoding;
        }

        public object? ProcessControlNode(byte bNodeType)
        {
            var controlType = (ControlType)bNodeType;
            switch (controlType)
            {
                case ControlType.NodeStart:
                    ProcessNodeStart();
                    break;
                case ControlType.Attribute:
                    ProcessAttribute();
                    break;
                case ControlType.NodeEnd:
                    if (HoldValue != null)
                    {
                        WriterProvider.WriteElementValue(HoldValue);
                        HoldValue = null;
                    }

                    WriterProvider.WriteEndElement();
                    break;
                case ControlType.FileEnd:
                    return WriterProvider.GetResult();
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return null;
        }

        public void ProcessDataNode(NodeType propertyType, bool isArray)
        {
            if (HoldValue != null)
            {
                WriterProvider.WriteElementValue(HoldValue);
                HoldValue = null;
            }

            var elementNameResult = NodeReader.ReadString();
            var elementName = elementNameResult.Value;
#if USELOG
            Logger.LogDataElement(elementName, elementNameResult.ReadStatus.Offset);
#endif
            WriterProvider.WriteStartElement(elementName);

            WriterProvider.WriteStartAttribute("__type");
            WriterProvider.WriteAttributeValue(propertyType.Name);
            WriterProvider.WriteEndAttribute();

            CurrentType = propertyType.Name;

            var arraySize = GetArraySize(propertyType, isArray);
            if (propertyType.Name == "str")
            {
                ProcessStringType(arraySize);
            }
            else if (propertyType.Name == "bin")
            {
                ProcessBinaryType(arraySize);
            }
            else
            {
                ProcessPrimitiveType(propertyType, isArray, arraySize);
            }
        }

        private void ProcessNodeStart()
        {
            if (HoldValue != null)
            {
                WriterProvider.WriteElementValue(HoldValue);
                HoldValue = null;
            }

            var elementNameResult = NodeReader.ReadString();
            var elementName = elementNameResult.Value;
#if USELOG
            Logger.LogStructElement(elementName, elementNameResult.ReadStatus.Offset);
#endif
            WriterProvider.WriteStartElement(elementName);
        }

        private void ProcessAttribute()
        {
            var attrResult = NodeReader.ReadString();
            var attr = attrResult.Value;
#if USELOG
            Logger.LogAttributeName(attr, attrResult.ReadStatus.Offset);
#endif
            var strLenResult = DataReader.ReadS32();
            var strLen = strLenResult.Value;
#if USELOG
            Logger.LogAttributeLength(strLen, strLenResult.ReadStatus.Offset, strLenResult.ReadStatus.Flag);
#endif
            var valueResult = DataReader.ReadString(strLen);
            var value = valueResult.Value;
#if USELOG
            Logger.LogAttributeValue(value, valueResult.ReadStatus.Offset, valueResult.ReadStatus.Flag);
#endif
            // Size has been written below
            if (CurrentType != "bin" || attr != "__size")
            {
                WriterProvider.WriteStartAttribute(attr);
                WriterProvider.WriteAttributeValue(value);
                WriterProvider.WriteEndAttribute();
            }
        }

        private void ProcessStringType(int arraySize)
        {
            var valueReadResult = DataReader.ReadString(arraySize);
            HoldValue = valueReadResult.Value;
#if USELOG
            Logger.LogStringValue(HoldValue, valueReadResult.ReadStatus.Offset, valueReadResult.ReadStatus.Flag);
#endif
        }

        private void ProcessBinaryType(int arraySize)
        {
            var upper = _readOptions.BinaryUppercase;
            WriterProvider.WriteStartAttribute("__size");
            WriterProvider.WriteAttributeValue(arraySize.ToString());
            WriterProvider.WriteEndAttribute();
            var valueReadResult = DataReader.ReadBinary(arraySize, upper);
            HoldValue = valueReadResult.Value;
#if USELOG
            Logger.LogBinaryValue(HoldValue, valueReadResult.ReadStatus.Offset, valueReadResult.ReadStatus.Flag);
#endif
        }

#if !NETSTANDARD2_0
        [SkipLocalsInit]
#endif
        private void ProcessPrimitiveType(NodeType propertyType, bool isArray, int arraySize)
        {
            if (isArray)
            {
                var size = (arraySize / (propertyType.Size * propertyType.Count)).ToString();
                WriterProvider.WriteStartAttribute("__count");
                WriterProvider.WriteAttributeValue(size);
                WriterProvider.WriteEndAttribute();
            }

            // force to read as 32bit if is array
            var spanResult = isArray
                ? DataReader.ReadBytes32BitAligned(arraySize)
                : DataReader.ReadBytes(arraySize);
            var span = spanResult.Span;

            Span<char> charSpan = stackalloc char[Constants.MaxStackLength];
            var stringBuilder = new ValueStringBuilder(charSpan);
            var loopCount = arraySize / propertyType.Size;
            for (var i = 0; i < loopCount; i++)
            {
                var subSpan = span.Slice(i * propertyType.Size, propertyType.Size);
#if NET8_0_OR_GREATER
                propertyType.SerializeAppend(ref stringBuilder, subSpan);
#else
                stringBuilder.Append(propertyType.Serialize(subSpan));
#endif
                if (i != loopCount - 1)
                {
#if NET8_0_OR_GREATER
                    stringBuilder.Append(' ');
#else
                    stringBuilder.Append(" ");
#endif
                }
            }

            HoldValue = stringBuilder.ToString();
#if USELOG
            Logger.LogArrayValue(HoldValue, spanResult.ReadStatus.Offset, spanResult.ReadStatus.Flag);
#endif
        }

        private int GetArraySize(NodeType propertyType, bool isArray)
        {
            int arraySize;
            if (isArray || propertyType.Name is "str" or "bin")
            {
                var valueReadResult = DataReader.ReadS32();
                arraySize = valueReadResult.Value; // Total size.
#if USELOG
                Logger.LogArraySize(arraySize, valueReadResult.ReadStatus.Offset, valueReadResult.ReadStatus.Flag);
#endif
            }
            else
            {
                arraySize = propertyType.Size * propertyType.Count;
            }

            return arraySize;
        }

        public void Dispose()
        {
            WriterProvider.Dispose();
        }
    }
}