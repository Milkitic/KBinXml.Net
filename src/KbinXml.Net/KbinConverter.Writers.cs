using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net.Internal;
using KbinXml.Net.Internal.Writers;
using KbinXml.Net.Utils;

namespace KbinXml.Net;

public static partial class KbinConverter
{
    #region byte[] returning methods

    /// <summary>
    /// Converts an XML document to KBin-formatted binary data.
    /// </summary>
    /// <param name="xml">The XML document to convert.</param>
    /// <param name="knownEncodings">The text encoding specification for the output KBin data.</param>
    /// <param name="writeOptions">Configuration options for the conversion process.</param>
    /// <returns>A byte array containing the KBin-formatted data.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="xml"/> is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="knownEncodings"/> specifies an unsupported encoding.</exception>
    /// <exception cref="KbinException">Invalid XML structure or data conversion error occurs.</exception>
    /// <remarks>
    /// <para>This method supports both compressed and uncompressed KBin formats.</para>
    /// <para>If <paramref name="writeOptions"/> is null, default options will be used.</para>
    /// </remarks>
    public static byte[] Write(XmlDocument xml, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
    {
        using var ms = RecyclableMemoryStreamManager.GetStream("byte[] returning methods");
        Write(xml, ms, knownEncodings, writeOptions);
        return ms.ToArray();
    }

    /// <summary>
    /// Converts a LINQ-to-XML element/document to KBin-formatted binary data.
    /// </summary>
    /// <param name="xml">The XML element or document to convert. Must be a valid <see cref="XContainer"/> (XElement or XDocument).</param>
    /// <param name="knownEncodings">The text encoding specification for the output KBin data. See supported values in <see cref="KnownEncodings"/>.</param>
    /// <param name="writeOptions">Configuration options for serialization. When null, uses default compression and validation settings.</param>
    /// <returns>A byte array containing structured KBin data with proper section alignment.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="xml"/> contains a null reference.</exception>
    /// <inheritdoc cref="Write(XmlDocument, KnownEncodings, WriteOptions?)"/>
    public static byte[] Write(XContainer xml, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
    {
        using var ms = RecyclableMemoryStreamManager.GetStream("byte[] returning methods");
        Write(xml, ms, knownEncodings, writeOptions);
        return ms.ToArray();
    }

    /// <summary>
    /// Converts XML text to KBin-formatted binary data.
    /// </summary>
    /// <param name="xmlText">The XML string to convert. Must be well-formed XML 1.0 text.</param>
    /// <param name="knownEncodings">The character encoding scheme for text conversion. Affects string storage in KBin format.</param>
    /// <param name="writeOptions">Serialization control parameters. Null values enable default compression and error handling behavior.</param>
    /// <returns>A byte array containing the KBin binary output with proper header validation.</returns>
    /// <exception cref="ArgumentException"><paramref name="xmlText"/> contains invalid XML syntax.</exception>
    /// <inheritdoc cref="Write(XmlDocument, KnownEncodings, WriteOptions?)"/>
    public static byte[] Write(string xmlText, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
    {
        using var ms = RecyclableMemoryStreamManager.GetStream("byte[] returning methods");
        Write(xmlText, ms, knownEncodings, writeOptions);
        return ms.ToArray();
    }

    /// <summary>
    /// Converts UTF-8 encoded XML bytes to KBin-formatted binary data.
    /// </summary>
    /// <param name="xmlBytes">The XML data to convert. Must be valid UTF-8 encoded bytes (with or without BOM).</param>
    /// <param name="knownEncodings">The target text encoding specification. Determines how strings are stored in the KBin output.</param>
    /// <param name="writeOptions">Serialization configuration parameters. Controls compression and validation behavior.</param>
    /// <returns>A byte array containing the complete KBin structure with node and data sections.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="xmlBytes"/> is a null reference.</exception>
    /// <inheritdoc cref="Write(XmlDocument, KnownEncodings, WriteOptions?)"/>
    public static byte[] Write(byte[] xmlBytes, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
    {
        using var ms = RecyclableMemoryStreamManager.GetStream("byte[] returning methods");
        Write(xmlBytes, ms, knownEncodings, writeOptions);
        return ms.ToArray();
    }

    #endregion

    #region Stream-based writing methods

    /// <summary>
    /// Converts an XML document to KBin-formatted binary data and writes it to a stream.
    /// </summary>
    /// <param name="xml">The XML document to convert.</param>
    /// <param name="outputStream">The stream to write the KBin data to.</param>
    /// <param name="knownEncodings">The text encoding specification for the output KBin data.</param>
    /// <param name="writeOptions">Configuration options for the conversion process.</param>
    /// <returns>The total number of bytes written to the stream.</returns>
    public static int Write(XmlDocument xml, Stream outputStream, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
    {
        if (xml is null)
            throw new ArgumentNullException(nameof(xml));
        if (outputStream is null)
            throw new ArgumentNullException(nameof(outputStream));

        using XmlReader reader = new XmlNodeReader(xml);
        return WriteCore(reader, outputStream, knownEncodings, writeOptions);
    }

    /// <summary>
    /// Converts a LINQ-to-XML element/document to KBin-formatted binary data and writes it to a stream.
    /// </summary>
    /// <param name="xml">The XML element or document to convert.</param>
    /// <param name="outputStream">The stream to write the KBin data to.</param>
    /// <param name="knownEncodings">The text encoding specification for the output KBin data.</param>
    /// <param name="writeOptions">Configuration options for serialization.</param>
    /// <returns>The total number of bytes written to the stream.</returns>
    public static int Write(XContainer xml, Stream outputStream, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
    {
        if (xml is null)
            throw new ArgumentNullException(nameof(xml));
        if (outputStream is null)
            throw new ArgumentNullException(nameof(outputStream));

        using var reader = xml.CreateReader();
        return WriteCore(reader, outputStream, knownEncodings, writeOptions);
    }

    /// <summary>
    /// Converts XML text to KBin-formatted binary data and writes it to a stream.
    /// </summary>
    /// <param name="xmlText">The XML string to convert.</param>
    /// <param name="outputStream">The stream to write the KBin data to.</param>
    /// <param name="knownEncodings">The character encoding scheme for text conversion.</param>
    /// <param name="writeOptions">Serialization control parameters.</param>
    /// <returns>The total number of bytes written to the stream.</returns>
    public static int Write(string xmlText, Stream outputStream, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
    {
        if (xmlText is null)
            throw new ArgumentNullException(nameof(xmlText));
        if (outputStream is null)
            throw new ArgumentNullException(nameof(outputStream));

        using var textReader = new StringReader(xmlText);
        using var reader = XmlReader.Create(textReader, new XmlReaderSettings { IgnoreWhitespace = true });
        return WriteCore(reader, outputStream, knownEncodings, writeOptions);
    }

    /// <summary>
    /// Converts UTF-8 encoded XML bytes to KBin-formatted binary data and writes it to a stream.
    /// </summary>
    /// <param name="xmlBytes">The XML data to convert.</param>
    /// <param name="outputStream">The stream to write the KBin data to.</param>
    /// <param name="knownEncodings">The target text encoding specification.</param>
    /// <param name="writeOptions">Serialization configuration parameters.</param>
    /// <returns>The total number of bytes written to the stream.</returns>
    public static int Write(byte[] xmlBytes, Stream outputStream, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
    {
        if (xmlBytes is null)
            throw new ArgumentNullException(nameof(xmlBytes));
        if (outputStream is null)
            throw new ArgumentNullException(nameof(outputStream));

        using var readerStream = new MemoryStream(xmlBytes);
        using var reader = XmlReader.Create(readerStream, new XmlReaderSettings { IgnoreWhitespace = true });
        return WriteCore(reader, outputStream, knownEncodings, writeOptions);
    }

    #endregion

    /// <summary>
    /// Central writing logic that handles context creation and disposal.
    /// </summary>
    private static int WriteCore(XmlReader reader, Stream outputStream, KnownEncodings knownEncodings, WriteOptions? writeOptions)
    {
        var encoding = knownEncodings.ToEncoding();
        writeOptions ??= new WriteOptions();
        var context = new WriteContext(new NodeWriter(writeOptions.Compress, encoding), new DataWriter(encoding), writeOptions);

        try
        {
            return WriterImpl(outputStream, encoding, ref context, reader, writeOptions);
        }
        finally
        {
            context.Dispose();
            context.DataWriter.Dispose();
            context.NodeWriter.Dispose();
        }
    }

    private static int WriterImpl(Stream outputStream, Encoding encoding, ref WriteContext context, XmlReader reader,
        WriteOptions writeOptions)
    {
        if (!EncodingDictionary.ReverseEncodingMap.TryGetValue(encoding, out var encodingBytes))
        {
            throw new ArgumentOutOfRangeException(nameof(encoding), encoding, "Unsupported encoding for KBin");
        }

        var repairedPrefix = writeOptions.RepairedPrefix;

        var nameTable = reader.NameTable;
        context.NameType = nameTable.Add("__type");
        context.NameCount = nameTable.Add("__count");
        context.NameSize = nameTable.Add("__size");

        while (reader.Read())
        {
            ProcessSingleRead(ref context, reader, repairedPrefix);
        }

        context.FlushPendingData();

        context.NodeWriter.WriteU8(255);
        context.NodeWriter.Pad();
        context.DataWriter.PadStream();

        return FinalizeOutput(outputStream, ref context, encodingBytes);
    }

    private static void ProcessSingleRead(ref WriteContext context, XmlReader reader, string? repairedPrefix)
    {
        switch (reader.NodeType)
        {
            case XmlNodeType.Element:
                context.FlushPendingData();

                if (reader.AttributeCount > 0)
                {
                    ProcessAttributes(reader, ref context, repairedPrefix);
                }

                if (context.Type.IsEmpty)
                {
                    context.NodeWriter.WriteU8(1);
                }
                else
                {
                    if (!NodeTypeFactory.TryGetNodeTypeId(context.Type.Span, out var typeId)) // 内部为字典操作
                    {
                        throw new KbinTypeNotFoundException(context.Type.ToString());
                    }

                    context.TypeId = typeId;
                    if (context.ArrayCountStr != null)
                    {
                        context.NodeWriter.WriteU8((byte)(context.TypeId | 0x40));
                    }
                    else
                    {
                        context.NodeWriter.WriteU8(context.TypeId);
                    }
                }

                var readerName = reader.Name;
                context.NodeWriter.WriteString(GetActualName(readerName, repairedPrefix));

                if (reader.IsEmptyElement)
                {
                    context.FlushPendingData();
                    context.NodeWriter.WriteU8(0xFE);
                }

                break;
            case XmlNodeType.Text:
                context.PendingValue = reader.Value;
                break;
            case XmlNodeType.EndElement:
                context.FlushPendingData();
                context.NodeWriter.WriteU8(0xFE);
                break;
            default:
                //Console.WriteLine("Other node {0} with value {1}",
                //    reader.NodeType, reader.Value);
                break;
        }
    }

    private static void ProcessAttributes(XmlReader reader, ref WriteContext context, string? repairedPrefix)
    {
        while (reader.MoveToNextAttribute())
        {
            if (reader.Prefix.Length > 0)
            {
                context.PendingAttributes.Add(
                    new KeyValuePair<string, string>(GetActualName(reader.Name, repairedPrefix), reader.Value));
                continue;
            }

            var localNameRef = reader.LocalName;
            if (ReferenceEquals(localNameRef, context.NameType))
            {
                context.ReadTypeValue(reader);
            }
            else if (ReferenceEquals(localNameRef, context.NameCount))
            {
                context.ArrayCountStr = reader.Value;
            }
            else if (ReferenceEquals(localNameRef, context.NameSize))
            {
            }
            else
            {
                context.PendingAttributes.Add(
                    new KeyValuePair<string, string>(GetActualName(reader.Name, repairedPrefix), reader.Value));
            }
        }

        reader.MoveToElement();
    }

    private static int FinalizeOutput(Stream outputStream, ref WriteContext context, byte encodingBytes)
    {
        var nodeLength = (int)context.NodeWriter.Stream.Length;
        var dataLength = (int)context.DataWriter.Stream.Length;

        var position = outputStream.Position;
        using var output = new BigEndianWriter(outputStream);

        //Write header data
        output.WriteU8(0xA0); // Signature
        output.WriteU8((byte)(context.NodeWriter.Compressed ? 0x42 : 0x45)); // Compression flag
        output.WriteU8(encodingBytes);
        output.WriteU8((byte)~encodingBytes);

        //Write node buffer length and contents.
        output.WriteS32(nodeLength);
        context.NodeWriter.Stream.WriteTo(outputStream);

        //Write data buffer length and contents.
        output.WriteS32(dataLength);
        context.DataWriter.Stream.WriteTo(outputStream);

        // Calculate total bytes written: Header (4) + NodeLen (4) + NodeData + DataLen (4) + Data
        return (int)(outputStream.Position - position);
    }

    private ref struct WriteContext : IDisposable
    {
        private readonly char[] _typeBuffer = ArrayPool<char>.Shared.Rent(4096);

        public readonly WriteOptions WriteOptions;
        public readonly List<KeyValuePair<string, string>> PendingAttributes;
        public NodeWriter NodeWriter;
        public DataWriter DataWriter;

        public ReadOnlyMemory<char> Type;

        public string? NameType;
        public string? NameCount;
        public string? NameSize;

        public string PendingValue;

        public string? ArrayCountStr;
        public byte TypeId = 0;

        public WriteContext(NodeWriter nodeWriter, DataWriter dataWriter, WriteOptions writeOptions)
        {
            NodeWriter = nodeWriter;
            DataWriter = dataWriter;
            WriteOptions = writeOptions;

            PendingAttributes = new List<KeyValuePair<string, string>>(8); // 预分配一个合理容量
            PendingValue = string.Empty;
            //TypeStr = null;
            ArrayCountStr = null;
            //TypeId = 0;
        }

        public void ReadTypeValue(XmlReader reader)
        {
            if (reader.CanReadValueChunk)
            {
                var length = reader.ReadValueChunk(_typeBuffer, 0, _typeBuffer.Length);
                Type = _typeBuffer.AsMemory(0, length);
            }
            else
            {
                Type = reader.Value.AsMemory();
            }
        }

        public void FlushPendingData()
        {
            if (!Type.IsEmpty)
            {
                ProcessTypeData();
            }

            if (PendingAttributes.Count > 0)
            {
                ProcessAttributes();
            }
        }

        private void ProcessTypeData()
        {
            // 使用switch提高性能
            switch (Type.Span)
            {
                case "str":
                    DataWriter.WriteString(PendingValue);
                    break;
                case "bin":
                    DataWriter.WriteBinary(PendingValue);
                    break;
                default:
                    ProcessComplexTypeData();
                    break;
            }

            // 重置状态
            ArrayCountStr = null;
            PendingValue = string.Empty;
            TypeId = 0;
            Type = ReadOnlyMemory<char>.Empty;
        }

        private void ProcessComplexTypeData()
        {
            var type = NodeTypeFactory.GetNodeType(TypeId);
            var valueEnumerator = PendingValue.SpanSplit(' '); // 已优化为Span操作

            var typeSize = type.Size;
            var requiredBytes = (uint)(typeSize * type.Count);
            if (ArrayCountStr != null)
            {
                if (!uint.TryParse(ArrayCountStr, out var count))
                {
                    throw new KbinException($"Invalid array count: {ArrayCountStr}");
                }

                requiredBytes *= count;
                DataWriter.WriteU32(requiredBytes);
            }

            if (requiredBytes > int.MaxValue)
            {
                throw new KbinException("Required bytes exceed maximum array size");
            }

            var iRequiredBytes = (int)requiredBytes;
            // 避免小数组的堆分配
            byte[]? arr = null;
            var span = iRequiredBytes <= Constants.MaxStackLength
                ? stackalloc byte[iRequiredBytes]
                : (arr = ArrayPool<byte>.Shared.Rent(iRequiredBytes)).AsSpan(0, iRequiredBytes);

            var builder = new ValueListBuilder<byte>(span);

            try
            {
                int bytesWritten = 0;
                var strictMode = WriteOptions.StrictMode;

                if (PendingValue.Length == 0 && strictMode && iRequiredBytes > 0)
                    throw new KbinException($"Node requires {iRequiredBytes} bytes but has no text value.");

                foreach (var s in valueEnumerator)
                {
                    try
                    {
                        if (bytesWritten == iRequiredBytes)
                        {
                            if (strictMode)
                            {
                                throw new KbinArrayCountMissMatchException(ArrayCountStr,
                                    PendingValue.Split(' ').Length);
                            }

                            break;
                        }

                        var add = type.WriteString(ref builder, s);
                        if (add < typeSize)
                        {
                            builder.AppendZeros(typeSize - add);
                        }

                        bytesWritten += typeSize;
                    }
                    catch (Exception e)
                    {
                        throw new KbinException(
                            $"Error while writing data '{s.ToString()}'. See InnerException for more information.",
                            e);
                    }
                }

                // 处理可能的字节数不足情况
                if (bytesWritten != iRequiredBytes)
                {
                    if (strictMode)
                    {
                        throw new KbinArrayCountMissMatchException(ArrayCountStr, builder.Length / typeSize);
                    }

                    // 填充剩余字节
                    builder.AppendZeros(iRequiredBytes - bytesWritten);
                }

                // 根据是否为数组选择合适的写入方法
                // If array, force write 32bit
                var builderSpan = builder.AsSpan();
                if (ArrayCountStr != null)
                {
                    DataWriter.Write32BitAligned(builderSpan);
                }
                else
                {
                    DataWriter.WriteBytes(builderSpan);
                }
            }
            finally
            {
                builder.Dispose();
                if (arr != null) ArrayPool<byte>.Shared.Return(arr);
            }
        }

        private void ProcessAttributes()
        {
            // Xml Attribute排序
            PendingAttributes.Sort(static (a, b) => string.Compare(a.Key, b.Key, StringComparison.Ordinal));

            foreach (var attribute in PendingAttributes)
            {
                NodeWriter.WriteU8(0x2E);
                NodeWriter.WriteString(attribute.Key);
                DataWriter.WriteString(attribute.Value);
            }

            PendingAttributes.Clear();
        }

        public void Dispose()
        {
            ArrayPool<char>.Shared.Return(_typeBuffer);
        }
    }
}