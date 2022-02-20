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
    /// Represents a reader for Konami's binary XML format.
    /// </summary>
    public sealed class XmlReader : IDisposable
    {
        private static readonly Type TypeControlType = typeof(ControlType);

        private static readonly HashSet<byte> _controlTypes =
            new HashSet<byte>(Enum.GetValues(TypeControlType).Cast<byte>());

        public Encoding Encoding { get; }

        private readonly NodeBuffer _nodeBuffer;
        private readonly DataBuffer _dataBuffer;

        private readonly System.Xml.XmlWriter _xmlWriter;
        private readonly Stream _writerStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="XmlReader"/> class.
        /// </summary>
        /// <param name="buffer">An array of bytes containing the contents of a binary XML.</param>
        public XmlReader(byte[] buffer)
        {
            //Read header section.
            var binaryBuffer = new BigEndianBinaryBuffer(buffer);
            var signature = binaryBuffer.ReadU8();
            var compressionFlag = binaryBuffer.ReadU8();
            var encodingFlag = binaryBuffer.ReadU8();
            var encodingFlagNot = binaryBuffer.ReadU8();

            //Verify magic.
            if (signature != 0xA0)
                throw new KbinException($"Signature was invalid. 0x{signature.ToString("X2")} != 0xA0");

            //Encoding flag should be an inverse of the fourth byte.
            if ((byte)~encodingFlag != encodingFlagNot)
                throw new KbinException($"Third byte was not an inverse of the fourth. {~encodingFlag} != {encodingFlagNot}");

            var compressed = compressionFlag == 0x42;
            Encoding = EncodingDictionary.EncodingMap[encodingFlag];

            //Get buffer lengths and load.
            var span = new Span<byte>(buffer);
            var nodeLength = binaryBuffer.ReadS32();
            _nodeBuffer = new NodeBuffer(span.Slice(8, nodeLength).ToArray(), compressed, Encoding);

            var dataLength = BitConverterHelper.GetBigEndianInt32(span.Slice(nodeLength + 8, 4));
            _dataBuffer = new DataBuffer(span.Slice(nodeLength + 12, dataLength).ToArray(), Encoding);

            var settings = new XmlWriterSettings
            {
                Async = false,
                Encoding = Encoding,
                Indent = false
            };
            _writerStream = new MemoryStream();
            _xmlWriter = System.Xml.XmlWriter.Create(_writerStream, settings);
            _xmlWriter.WriteStartDocument();
        }

        /// <summary>
        /// Reads stream from the binary XML.
        /// </summary>
        /// <returns>Returns the XmlReader.</returns>
        public System.Xml.XmlReader ReadStream()
        {
            var bytes = ReadXmlByte();
            using (var memoryStream = new MemoryStream(bytes))
            {
                var xmlReader = System.Xml.XmlReader.Create(memoryStream, new XmlReaderSettings());
                return xmlReader;
            }
        }

        /// <summary>
        /// Reads all nodes in the binary XML.
        /// </summary>
        /// <returns>Returns the XDocument.</returns>
        public XDocument ReadLinq()
        {
            var bytes = ReadXmlByte();
            using (var memoryStream = new MemoryStream(bytes))
            {
                return XDocument.Load(memoryStream);
            }
        }


        /// <summary>
        /// Reads all nodes in the binary XML.
        /// </summary>
        /// <returns>Returns the XmlDocument.</returns>
        [Obsolete("Poor performance. Use \"" + nameof(ReadLinq) + "()\" instead.")]
        public XmlDocument Read()
        {
            var bytes = ReadXmlByte();
            using (var memoryStream = new MemoryStream(bytes))
            {
                var xmlElement = new XmlDocument();
                xmlElement.Load(memoryStream);
                return xmlElement;
            }
        }

        public byte[] ReadXmlByte()
        {
            string holdValue = null;
            while (true)
            {
                var nodeType = _nodeBuffer.ReadU8();

                //Array flag is on the second bit
                var array = (nodeType & 0x40) > 0;
                nodeType = (byte)(nodeType & ~0x40);
                NodeType propertyType;

                if (_controlTypes.Contains(nodeType))
                {
                    switch ((ControlType)nodeType)
                    {
                        case ControlType.NodeStart:
                            if (holdValue != null)
                            {
                                _xmlWriter.WriteString(holdValue);
                                holdValue = null;
                            }

                            var elementName = _nodeBuffer.ReadString();
                            _xmlWriter.WriteStartElement(elementName);
                            break;

                        case ControlType.Attribute:
                            var attr = _nodeBuffer.ReadString();
                            var value = _dataBuffer.ReadString(_dataBuffer.ReadS32());
                            _xmlWriter.WriteStartAttribute(attr);
                            _xmlWriter.WriteString(value);
                            _xmlWriter.WriteEndAttribute();
                            break;

                        case ControlType.NodeEnd:
                            if (holdValue != null)
                            {
                                _xmlWriter.WriteString(holdValue);
                                holdValue = null;
                            }

                            _xmlWriter.WriteEndElement();
                            break;

                        case ControlType.FileEnd:
                            _xmlWriter.Flush();
                            return _writerStream.ToArray();
                    }
                }
                else if (TypeDictionary.TypeMap.TryGetValue(nodeType, out propertyType))
                {
                    if (holdValue != null)
                    {
                        _xmlWriter.WriteString(holdValue);
                        holdValue = null;
                    }

                    var elementName = _nodeBuffer.ReadString();
                    _xmlWriter.WriteStartElement(elementName);

                    _xmlWriter.WriteStartAttribute("__type");
                    _xmlWriter.WriteString(propertyType.Name);
                    _xmlWriter.WriteEndAttribute();

                    int arraySize;
                    if (array || propertyType.Name == "str" || propertyType.Name == "bin")
                        arraySize = _dataBuffer.ReadS32(); //Total size.
                    else
                        arraySize = propertyType.Size * propertyType.Count;

                    if (propertyType.Name == "str")
                        holdValue = _dataBuffer.ReadString(arraySize);
                    else if (propertyType.Name == "bin")
                    {
                        _xmlWriter.WriteStartAttribute("__size");
                        _xmlWriter.WriteString(arraySize.ToString());
                        _xmlWriter.WriteEndAttribute();
                        holdValue = _dataBuffer.ReadBinary(arraySize);
                    }
                    else
                    {
                        if (array)
                        {
                            var size = (arraySize / (propertyType.Size * propertyType.Count)).ToString();
                            _xmlWriter.WriteStartAttribute("__count");
                            _xmlWriter.WriteString(size);
                            _xmlWriter.WriteEndAttribute();
                        }

                        var span = _dataBuffer.ReadBytes(arraySize);
                        var stringBuilder = new StringBuilder();
                        var loopCount = arraySize / propertyType.Size;
                        for (var i = 0; i < loopCount; i++)
                        {
                            var subSpan = span.Slice(i * propertyType.Size, propertyType.Size);
                            stringBuilder.Append(propertyType.GetString(subSpan));
                            if (i != loopCount - 1) stringBuilder.Append(" ");
                        }

                        holdValue = stringBuilder.ToString();
                    }
                }
                else
                {
                    throw new KbinException($"Unknown node type: {nodeType}");
                }
            }
        }

        public void Dispose()
        {
            _nodeBuffer?.Dispose();
            _dataBuffer?.Dispose();
            _xmlWriter?.Dispose();
            _writerStream?.Dispose();
        }
    }
}
