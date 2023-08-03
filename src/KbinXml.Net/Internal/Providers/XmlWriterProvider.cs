using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml;

namespace KbinXml.Net.Internal.Providers;

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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteStartDocument()
    {
        _xmlWriter.WriteStartDocument();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteElementValue(string? value)
    {
        _xmlWriter.WriteString(value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteAttributeValue(string? value)
    {
        _xmlWriter.WriteString(value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteStartElement(string value)
    {
        _xmlWriter.WriteStartElement(value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteEndElement()
    {
        _xmlWriter.WriteEndElement();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteStartAttribute(string value)
    {
        _xmlWriter.WriteStartAttribute(value);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteEndAttribute()
    {
        _xmlWriter.WriteEndAttribute();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override object GetResult()
    {
        _xmlWriter.Flush();
        return _writerStream.ToArray();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void Dispose()
    {
        _xmlWriter.Dispose();
        _writerStream.Dispose();
    }
}