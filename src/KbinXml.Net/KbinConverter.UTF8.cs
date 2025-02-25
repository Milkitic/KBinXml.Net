using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using KbinXml.Net.Internal;
using KbinXml.Net.Utils;
using KbinXml.Net.Writers;
using U8Xml;

namespace KbinXml.Net;

public static partial class KbinConverter
{

    /// <summary>
    /// Converts the provided XML to KBin bytes.
    /// </summary>
    /// <param name="xmlText">The XML text to convert.</param>
    /// <param name="writeOptions">Set the write options for writing.</param>
    /// <returns>The bytes of KBin.</returns>
    public static byte[] WriteU8(string xmlText, WriteOptions? writeOptions = null)
    {
        writeOptions ??= new WriteOptions();
        var context = new WriteContext(new NodeWriter(writeOptions.Compress, Encoding.UTF8), new DataWriter(Encoding.UTF8));

        using var textReader = new StringReader(xmlText);
        using var xml = XmlParser.Parse(xmlText);
        //using var reader = XmlReader.Create(textReader, new XmlReaderSettings { IgnoreWhitespace = true });

        try
        {
            return WriterImplU8(context, xml, writeOptions);
        }
        finally
        {
            context.DataWriter.Dispose();
            context.NodeWriter.Dispose();
        }
    }

    /// <summary>
    /// Converts the provided XML to KBin bytes.
    /// </summary>
    /// <param name="xmlBytes">The XML bytes to convert.</param>
    /// <param name="writeOptions">Set the write options for writing.</param>
    /// <returns>The bytes of KBin.</returns>
    public static byte[] WriteU8(byte[] xmlBytes, WriteOptions? writeOptions = null)
    {
        writeOptions ??= new WriteOptions();
        var context = new WriteContext(new NodeWriter(writeOptions.Compress, Encoding.UTF8), new DataWriter(Encoding.UTF8));

        using var ms = new MemoryStream(xmlBytes);
        using var xml = XmlParser.Parse(ms);

        try
        {
            return WriterImplU8(context, xml, writeOptions);
        }
        finally
        {
            context.DataWriter.Dispose();
            context.NodeWriter.Dispose();
        }
    }


    private static byte[] WriterImplU8(WriteContext context, XmlObject reader,
        WriteOptions writeOptions)
    {
        var holdingAttrs = new SortedDictionary<string, string>(StringComparer.Ordinal);
        string holdingValue = "";
        string? typeStr = null;
        string? arrayCountStr = null;
        byte typeid = 0;

        void EnsureHolding()
        {
            if (typeStr != null)
            {
                if (typeStr == "str")
                    context.DataWriter.WriteString(holdingValue);
                else if (typeStr == "bin")
                    context.DataWriter.WriteBinary(holdingValue);
                else
                {
                    var type = TypeDictionary.TypeMap[typeid];
                    var value = holdingValue.SpanSplit(' ');
                    var requiredBytes = (uint)(type.Size * type.Count);
                    if (arrayCountStr != null)
                    {
                        requiredBytes *= uint.Parse(arrayCountStr);
                        context.DataWriter.WriteU32(requiredBytes);
                    }

                    if (requiredBytes > int.MaxValue)
                        throw new KbinException("uint size is greater than int.MaxValue");

                    var iRequiredBytes = (int)requiredBytes;
                    byte[]? arr = null;
                    var span = iRequiredBytes <= Constants.MaxStackLength
                        ? stackalloc byte[iRequiredBytes]
                        : arr = ArrayPool<byte>.Shared.Rent(iRequiredBytes);

                    if (arr != null) span = span.Slice(0, iRequiredBytes);
                    var builder = new ValueListBuilder<byte>(span);

                    try
                    {
                        int i = 0;
                        foreach (var s in value)
                        {
                            try
                            {
                                if (i == iRequiredBytes)
                                {
                                    if (writeOptions.StrictMode)
                                        throw new ArgumentOutOfRangeException("Length", holdingValue.Split(' ').Length,
                                            "The array length doesn't match the \"__count\" attribute. Expect: " +
                                            arrayCountStr);
                                    break;
                                }

                                var add = type.WriteString(ref builder, s);
                                if (add < type.Size)
                                {
                                    var left = type.Size - add;
                                    for (var j = 0; j < left; j++) builder.Append(0);
                                }

                                i += type.Size;
                            }
                            catch (Exception e)
                            {
                                throw new KbinException(
                                    $"Error while writing data '{s.ToString()}'. See InnerException for more information.",
                                    e);
                            }
                        }

                        if (i != requiredBytes)
                        {
                            if (writeOptions.StrictMode)
                                throw new ArgumentOutOfRangeException("Length", builder.Length / type.Size,
                                    "The array length doesn't match the \"__count\" attribute. Expect: " +
                                    arrayCountStr);

                            while (i != requiredBytes)
                            {
                                builder.Append(0);
                                i++;
                            }
                        }

                        // force to write as 32bit if is array
                        if (arrayCountStr != null)
                            context.DataWriter.Write32BitAligned(builder.AsSpan());
                        else
                            context.DataWriter.WriteBytes(builder.AsSpan());
                    }
                    finally
                    {
                        builder.Dispose();
                        if (arr != null) ArrayPool<byte>.Shared.Return(arr);
                    }
                }

                typeStr = null;
                arrayCountStr = null;
                holdingValue = "";
                typeid = 0;
            }

            if (holdingAttrs.Count > 0)
            {
                foreach (var attribute in holdingAttrs)
                {
                    context.NodeWriter.WriteU8(0x2E);
                    context.NodeWriter.WriteString(attribute.Key);
                    context.DataWriter.WriteString(attribute.Value);
                }

                holdingAttrs.Clear();
            }
        }

        foreach (var child in reader.Root.Children)
        {

        }

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    EnsureHolding();
                    //Console.WriteLine("Start Element {0}", reader.Name);
                    if (reader.AttributeCount > 0)
                    {
                        for (int i = 0; i < reader.AttributeCount; i++)
                        {
                            reader.MoveToAttribute(i);
                            if (reader.Name == "__type")
                            {
                                typeStr = reader.Value;
                            }
                            else if (reader.Name == "__count")
                            {
                                arrayCountStr = reader.Value;
                            }
                            else if (reader.Name == "__size")
                            {
                                // ignore
                            }
                            else
                            {
                                holdingAttrs.Add(GetActualName(reader.Name, writeOptions.RepairedPrefix), reader.Value);
                            }
                        }

                        reader.MoveToElement();
                    }

                    if (typeStr == null)
                    {
                        context.NodeWriter.WriteU8(1);
                        context.NodeWriter.WriteString(GetActualName(reader.Name, writeOptions.RepairedPrefix));
                    }
                    else
                    {
                        typeid = TypeDictionary.ReverseTypeMap[typeStr];
                        if (arrayCountStr != null)
                            context.NodeWriter.WriteU8((byte)(typeid | 0x40));
                        else
                            context.NodeWriter.WriteU8(typeid);

                        context.NodeWriter.WriteString(GetActualName(reader.Name, writeOptions.RepairedPrefix));
                    }

                    if (reader.IsEmptyElement)
                    {
                        EnsureHolding();
                        context.NodeWriter.WriteU8(0xFE);
                    }

                    break;
                case XmlNodeType.Text:
                    holdingValue = reader.Value;
                    break;
                case XmlNodeType.EndElement:
                    EnsureHolding();
                    context.NodeWriter.WriteU8(0xFE);
                    break;
                default:
                    //Console.WriteLine("Other node {0} with value {1}",
                    //    reader.NodeType, reader.Value);
                    break;
            }
        }

        EnsureHolding();

        context.NodeWriter.WriteU8(255);
        context.NodeWriter.Pad();
        context.DataWriter.Pad();

        using var output = new BeBinaryWriter();
        //Write header data
        output.WriteU8(0xA0); // Signature
        output.WriteU8((byte)(context.NodeWriter.Compressed ? 0x42 : 0x45)); // Compression flag
        var encodingBytes = EncodingDictionary.ReverseEncodingMap[Encoding.UTF8];
        output.WriteU8(encodingBytes);
        output.WriteU8((byte)~encodingBytes);

        //Write node buffer length and contents.
        var nodeLength = context.NodeWriter.Stream.Length;
        output.WriteS32((int)nodeLength);
        context.NodeWriter.Stream.WriteTo(output.Stream);

        //Write data buffer length and contents.
        var dataLength = context.DataWriter.Stream.Length;
        output.WriteS32((int)dataLength);
        context.DataWriter.Stream.WriteTo(output.Stream);

        return output.ToArray();
    }
}