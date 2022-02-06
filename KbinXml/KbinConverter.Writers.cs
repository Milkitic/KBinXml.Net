using System;
using System.Buffers;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net.Internal;
using KbinXml.Net.Utils;
using KbinXml.Net.Writers;

namespace KbinXml.Net;

public static partial class KbinConverter
{
    public static byte[] Write(XNode xml, Encoding encoding)
    {
        var context = new WriteContext(new NodeWriter(true, encoding), new DataWriter(encoding));

        using var reader = xml.CreateReader();
        return WriterImpl(encoding, context, reader);
    }

    public static byte[] Write(string rawXml, Encoding encoding)
    {
        var context = new WriteContext(new NodeWriter(true, encoding), new DataWriter(encoding));

        using var textReader = new StringReader(rawXml);
        using var reader = XmlReader.Create(textReader, new XmlReaderSettings { IgnoreWhitespace = true });

        return WriterImpl(encoding, context, reader);
    }

    private static byte[] WriterImpl(Encoding encoding, WriteContext context, XmlReader reader)
    {
        var holdingAttrs = new SortedDictionary<string, string>(StringComparer.Ordinal);
        string holdingValue = "";
        string? typeStr = null;
        string? sizeStr = null;
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
                    var size = (uint)(type.Size * type.Count);
                    if (sizeStr != null)
                    {
                        size *= uint.Parse(sizeStr);
                        context.DataWriter.WriteU32(size);
                    }

                    if (size > int.MaxValue)
                        throw new KbinException("uint size is greater than int.MaxValue");

                    var iSize = (int)size;
                    byte[]? arr = null;
                    var span = iSize <= Constants.MaxStackLength
                        ? stackalloc byte[iSize]
                        : arr = ArrayPool<byte>.Shared.Rent(iSize);

                    if (arr != null) span = span.Slice(0, iSize);
                    var builder = new ValueListBuilder<byte>(span);

                    try
                    {
                        int i = 0;
                        foreach (var s in value)
                        {
                            if (i == iSize) break;
                            var add = type.WriteString(ref builder, s);
                            if (add < type.Size)
                            {
                                var left = type.Size - add;
                                for (var j = 0; j < left; j++) builder.Append(0);
                            }

                            i += type.Size;
                        }

                        context.DataWriter.WriteBytes(builder.AsSpan());
                    }
                    finally
                    {
                        builder.Dispose();
                        if (arr != null) ArrayPool<byte>.Shared.Return(arr);
                    }
                }

                typeStr = null;
                sizeStr = null;
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
                                sizeStr = reader.Value;
                            }
                            else
                            {
                                holdingAttrs.Add(reader.Name, reader.Value);
                            }
                        }

                        reader.MoveToElement();
                    }

                    if (typeStr == null)
                    {
                        context.NodeWriter.WriteU8(1);
                        context.NodeWriter.WriteString(reader.Name);
                    }
                    else
                    {
                        typeid = TypeDictionary.ReverseTypeMap[typeStr];
                        if (sizeStr != null)
                            context.NodeWriter.WriteU8((byte)(typeid | 0x40));
                        else
                            context.NodeWriter.WriteU8(typeid);

                        context.NodeWriter.WriteString(reader.Name);
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

        //Write header data
        var output = new BeBinaryWriter();
        output.WriteU8(0xA0); //Magic
        output.WriteU8(0x42); //Compression flag
        output.WriteU8(EncodingDictionary.ReverseEncodingMap[encoding]);
        output.WriteU8((byte)~EncodingDictionary.ReverseEncodingMap[encoding]);

        //Write node buffer length and contents.
        var nodeStream = context.NodeWriter.Stream;
        if (nodeStream.Length <= int.MaxValue)
        {
            nodeStream.Position = 0;

            var length = (int)nodeStream.Length;
            var arr = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                var read = nodeStream.Read(arr, 0, length);

                Debug.Assert(length == read);

                output.WriteS32(length);
                output.WriteBytes(arr.AsSpan(0, length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(arr);
            }
        }
        else
        {
            var buffer = context.NodeWriter.ToArray();
            output.WriteS32(buffer.Length);
            output.WriteBytes(buffer);
        }

        //Write data buffer length and contents.
        var dataStream = context.DataWriter.Stream;
        if (dataStream.Length <= int.MaxValue)
        {
            dataStream.Position = 0;

            var length = (int)dataStream.Length;
            var arr = ArrayPool<byte>.Shared.Rent(length);
            try
            {
                var read = dataStream.Read(arr, 0, length);

                Debug.Assert(length == read);

                output.WriteS32(length);
                output.WriteBytes(arr.AsSpan(0, length));
            }
            finally
            {
                ArrayPool<byte>.Shared.Return(arr);
            }
        }
        else
        {
            var array = context.DataWriter.ToArray();
            output.WriteS32(array.Length);
            output.WriteBytes(array);
        }

        return output.ToArray();
    }

    private class WriteContext
    {
        public WriteContext(NodeWriter nodeWriter, DataWriter dataWriter)
        {
            NodeWriter = nodeWriter;
            DataWriter = dataWriter;
        }

        public NodeWriter NodeWriter { get; set; }
        public DataWriter DataWriter { get; set; }
    }
}