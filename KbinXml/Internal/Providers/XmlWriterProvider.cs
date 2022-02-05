using System.IO;
using System.Text;
using System.Xml;

namespace KbinXml.Internal.Providers;

internal class XmlWriterProvider : WriterProvider
{
    private readonly MemoryStream _writerStream;
    private readonly XmlWriter _xmlWriter;

    public XmlWriterProvider(Encoding encoding) : base(encoding)
    {
        var settings = new XmlWriterSettings
        {
            Async = false,
            Encoding = encoding,
            Indent = false
        };
        _writerStream = new MemoryStream();
        _xmlWriter = XmlWriter.Create(_writerStream, settings);
    }

    public override void WriteStartDocument()
    {
        _xmlWriter.WriteStartDocument();
    }

    public override void WriteElementValue(string? value)
    {
        _xmlWriter.WriteString(value);
    }

    public override void WriteAttributeValue(string? value)
    {
        _xmlWriter.WriteString(value);
    }

    public override void WriteStartElement(string value)
    {
        _xmlWriter.WriteStartElement(value);
    }

    public override void WriteEndElement()
    {
        _xmlWriter.WriteEndElement();
    }

    public override void WriteStartAttribute(string value)
    {
        _xmlWriter.WriteStartAttribute(value);
    }

    public override void WriteEndAttribute()
    {
        _xmlWriter.WriteEndAttribute();
    }

    public override object GetResult()
    {
        _xmlWriter.Flush();
        return _writerStream.ToArray();
    }

    public override void Dispose()
    {
        _writerStream.Dispose();
        _xmlWriter.Dispose();
    }
}