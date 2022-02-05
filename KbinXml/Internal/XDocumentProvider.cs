using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;

namespace KbinXml.Internal;

internal class XDocumentProvider : WriterProvider
{
    private readonly XDocument _xDocument;
    private readonly Stack<XContainer> _nodeStack = new();

    private string? _holdAttrName;

    public XDocumentProvider(Encoding encoding) : base(encoding)
    {
        _xDocument = new XDocument(new XDeclaration("1.0", encoding.WebName, null));
        _nodeStack.Push(_xDocument);
    }

    public override void WriteStartDocument()
    {
    }

    public override void WriteElementValue(string? value)
    {
        var current = _nodeStack.Peek();
        current.Add(value);
    }

    public override void WriteAttributeValue(string? value)
    {
        if (_holdAttrName != null)
        {
            var current = _nodeStack.Peek();
            var attr = new XAttribute(XName.Get(_holdAttrName), value ?? "");
            current.Add(attr);
        }
        else throw new Exception("Current attribute is null!");
    }

    public override void WriteStartElement(string elementName)
    {
        var current = _nodeStack.Peek();

        var node = new XElement(elementName);
        current.Add(node);
        _nodeStack.Push(node);
    }

    public override void WriteEndElement()
    {
        _nodeStack.Pop();
    }

    public override void WriteStartAttribute(string value)
    {
        _holdAttrName = value;
    }

    public override void WriteEndAttribute()
    {
        _holdAttrName = null;
    }

    public override object GetResult()
    {
        return _xDocument;
    }
}