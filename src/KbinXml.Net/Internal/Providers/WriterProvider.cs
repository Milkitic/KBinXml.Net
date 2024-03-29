﻿using System;
using System.Text;

namespace KbinXml.Net.Internal.Providers;

internal abstract class WriterProvider : IDisposable
{
    protected readonly Encoding Encoding;

    public WriterProvider(Encoding encoding)
    {
        Encoding = encoding;
    }

    public abstract void WriteStartDocument();
    public abstract void WriteElementValue(string? value);
    public abstract void WriteAttributeValue(string? value);
    public abstract void WriteStartElement(string elementName);
    public abstract void WriteEndElement();
    public abstract void WriteStartAttribute(string value);
    public abstract void WriteEndAttribute();
    public abstract object GetResult();

    public virtual void Dispose()
    {
    }
}