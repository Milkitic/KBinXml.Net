using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace StableKbin
{
    /// <summary>
    /// Represents a writer for Konami's binary XML format.
    /// </summary>
    public sealed class XmlWriter : IDisposable
    {
        private readonly XDocument _document;
        private readonly Encoding _encoding;

        private readonly NodeBuffer _nodeBuffer;
        private readonly DataBuffer _dataBuffer;
        private string _rawXml;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWriter"/> class.
        /// </summary>
        /// <param name="xNode">The XML document to be written as a binary XML.</param>
        /// <param name="encoding">The encoding of the XML document.</param>
        public XmlWriter(XNode xNode, Encoding encoding)
        {
            _document = xNode is XDocument xDoc ? xDoc : new XDocument(xNode);

            _encoding = encoding;
            _nodeBuffer = new NodeBuffer(true, encoding);
            _dataBuffer = new DataBuffer(encoding);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlWriter"/> class.
        /// </summary>
        /// <param name="rawXml">The XML document to be written as a binary XML.</param>
        /// <param name="encoding">The encoding of the XML document.</param>
        public XmlWriter(string rawXml, Encoding encoding)
        {
            _rawXml = rawXml;

            _encoding = encoding;
            _nodeBuffer = new NodeBuffer(true, encoding);
            _dataBuffer = new DataBuffer(encoding);
        }

        /// <summary>
        /// Writes all nodes in the XML document.
        /// </summary>
        /// <returns>Returns an array of bytes containing the contents of the binary XML.</returns>
        public byte[] Write()
        {
            if (_rawXml != null)
            {
                return WriteRaw();
            }
            else
            {
                return WriteXml();
            }
        }

        private byte[] WriteRaw()
        {
            using (var textReader = new StringReader(_rawXml))
            using (var reader = System.Xml.XmlReader.Create(textReader, new XmlReaderSettings { IgnoreWhitespace = true }))
            {
                var holdingAttrs = new SortedDictionary<string, string>(StringComparer.Ordinal);
                string holdingValue = "";
                string typeStr = null, sizeStr = null;
                byte typeid = 0;

                void EnsureHolding()
                {
                    if (typeStr != null)
                    {
                        if (typeStr == "str")
                            _dataBuffer.WriteString(holdingValue);
                        else if (typeStr == "bin")
                            _dataBuffer.WriteBinary(holdingValue);
                        else
                        {
                            var type = TypeDictionary.TypeMap[typeid];
                            var value = holdingValue.SpanSplit(' ');
                            var size = (uint)(type.Size * type.Count);
                            if (sizeStr != null)
                            {
                                size *= uint.Parse(sizeStr);
                                _dataBuffer.WriteU32(size);
                            }

                            var arr = new byte[size];

                            int i = 0;
                            foreach (var s in value)
                            {
                                if (i == arr.Length) break;
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
                                var buffer = type.GetBytes(s);
                                buffer.CopyTo(arr.AsSpan().Slice(i, type.Size));
#elif NETSTANDARD2_0
                                var buffer = type.GetBytes(s).ToArray();
                                buffer.CopyTo(arr, i);
#endif
                                i += type.Size;
                            }

                            _dataBuffer.WriteBytes(arr);
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
                            _nodeBuffer.WriteU8(0x2E);
                            _nodeBuffer.WriteString(attribute.Key);
                            _dataBuffer.WriteString(attribute.Value);
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
                                _nodeBuffer.WriteU8(1);
                                _nodeBuffer.WriteString(reader.Name);
                            }
                            else
                            {
                                typeid = TypeDictionary.ReverseTypeMap[typeStr];
                                if (sizeStr != null)
                                    _nodeBuffer.WriteU8((byte)(typeid | 0x40));
                                else
                                    _nodeBuffer.WriteU8(typeid);

                                _nodeBuffer.WriteString(reader.Name);
                            }

                            if (reader.IsEmptyElement)
                            {
                                EnsureHolding();
                                _nodeBuffer.WriteU8(0xFE);
                            }

                            break;
                        case XmlNodeType.Text:
                            holdingValue = reader.Value;
                            break;
                        case XmlNodeType.EndElement:
                            EnsureHolding();
                            _nodeBuffer.WriteU8(0xFE);
                            break;
                        default:
                            //Console.WriteLine("Other node {0} with value {1}",
                            //    reader.NodeType, reader.Value);
                            break;
                    }
                }

                EnsureHolding();
            }

            _nodeBuffer.WriteU8(255);
            _nodeBuffer.Pad();
            _dataBuffer.Pad();

            //Write header data
            using (var output = new BigEndianBinaryBuffer())
            {
                output.WriteU8(0xA0); //Magic
                output.WriteU8(0x42); //Compression flag
                output.WriteU8(EncodingDictionary.ReverseEncodingMap[_encoding]);
                output.WriteU8((byte)~EncodingDictionary.ReverseEncodingMap[_encoding]);

                //Write node buffer length and contents.
                var buffer = _nodeBuffer.ToArray();
                output.WriteS32(buffer.Length);
                output.WriteBytes(buffer);

                //Write data buffer length and contents.
                var array = _dataBuffer.ToArray();
                output.WriteS32(array.Length);
                output.WriteBytes(array);

                return output.ToArray();
            }
        }

        private byte[] WriteXml()
        {
            XmlRecurse(_document.Root);
            _nodeBuffer.WriteU8(255);
            _nodeBuffer.Pad();
            _dataBuffer.Pad();

            //Write header data
            using (var output = new BigEndianBinaryBuffer())
            {
                output.WriteU8(0xA0); //Magic
                output.WriteU8(0x42); //Compression flag
                output.WriteU8(EncodingDictionary.ReverseEncodingMap[_encoding]);
                output.WriteU8((byte)~EncodingDictionary.ReverseEncodingMap[_encoding]);

                //Write node buffer length and contents.
                var buffer = _nodeBuffer.ToArray();
                output.WriteS32(buffer.Length);
                output.WriteBytes(buffer);

                //Write data buffer length and contents.
                var array = _dataBuffer.ToArray();
                output.WriteS32(array.Length);
                output.WriteBytes(array);

                return output.ToArray();
            }
        }

        private void XmlRecurse(XElement xElement)
        {
            var typeStr = xElement.Attribute("__type")?.Value;
            var sizeStr = xElement.Attribute("__count")?.Value;

            if (typeStr == null)
            {
                _nodeBuffer.WriteU8(1);
                _nodeBuffer.WriteString(xElement.Name.LocalName);
            }
            else
            {
                var typeid = TypeDictionary.ReverseTypeMap[typeStr];
                if (sizeStr != null)
                    _nodeBuffer.WriteU8((byte)(typeid | 0x40));
                else
                    _nodeBuffer.WriteU8(typeid);

                _nodeBuffer.WriteString(xElement.Name.LocalName);
                var innerText = xElement.Value;
                if (typeStr == "str")
                    _dataBuffer.WriteString(innerText);
                else if (typeStr == "bin")
                    _dataBuffer.WriteBinary(innerText);
                else
                {
                    var type = TypeDictionary.TypeMap[typeid];
                    var value = innerText.SpanSplit(' ');
                    var size = (uint)(type.Size * type.Count);

                    if (sizeStr != null)
                    {
                        size *= uint.Parse(sizeStr);
                        _dataBuffer.WriteU32(size);
                    }

                    var arr = new byte[size];

                    int i = 0;
                    foreach (var s in value)
                    {
                        if (i == arr.Length) break;
#if NETSTANDARD2_1 || NET5_0_OR_GREATER
                        var buffer = type.GetBytes(s);
                        buffer.CopyTo(arr.AsSpan().Slice(i, type.Size));
#elif NETSTANDARD2_0
                        var buffer = type.GetBytes(s).ToArray();
                        buffer.CopyTo(arr, i);
#endif
                        i += type.Size;
                    }

                    _dataBuffer.WriteBytes(arr);
                }
            }

            foreach (var attribute in xElement
                .Attributes()
                .Where(x => x.Name != "__type" && x.Name != "__size" && x.Name != "__count")
                .OrderBy(x => x.Name.LocalName))
            {
                _nodeBuffer.WriteU8(0x2E);
                _nodeBuffer.WriteString(attribute.Name.LocalName);
                _dataBuffer.WriteString(attribute.Value);
            }

            foreach (var childNode in xElement.Elements())
            {
                XmlRecurse(childNode);
            }

            _nodeBuffer.WriteU8(0xFE);
        }

        public void Dispose()
        {
            _nodeBuffer?.Dispose();
            _dataBuffer?.Dispose();
        }
    }
}
