using System;
using System.Text;

namespace KbinXml.Internal;

internal abstract class WriterProvider : IDisposable
{
    protected readonly Encoding Encoding;

    public WriterProvider(Encoding encoding)
    {
        Encoding = encoding;
    }

    public abstract void WriteStartDocument();
    public abstract void WriteString(string? value);
    public abstract void WriteStartElement(string elementName);
    public abstract void WriteEndElement();
    public abstract void WriteStartAttribute(string value);
    public abstract void WriteEndAttribute();
    public abstract object GetResult();

    public virtual void Dispose()
    {
    }
}