using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

namespace KbinXml.Net.Internal.Providers;

internal class XmlDocumentProvider : WriterProvider
{
    private readonly ReadOptions _readOptions;
    private readonly XmlDocument _xmlDocument;
    private readonly Stack<XmlNode> _nodeStack = new();

    private string? _holdAttrName;

    public XmlDocumentProvider(Encoding encoding, ReadOptions readOptions) : base(encoding)
    {
        _readOptions = readOptions;
        _xmlDocument = new XmlDocument();
        _xmlDocument.InsertBefore(_xmlDocument.CreateXmlDeclaration("1.0", Encoding.WebName, null),
            _xmlDocument.DocumentElement);
        _nodeStack.Push(_xmlDocument);
    }

    public override void WriteStartDocument()
    {
    }

    public override void WriteElementValue(string? value)
    {
        var current = _nodeStack.Peek();
        current.InnerText = value;
    }

    public override void WriteAttributeValue(string? value)
    {
        if (_holdAttrName != null)
        {
            var current = _nodeStack.Peek();
            var attr = _xmlDocument.CreateAttribute(_holdAttrName);
            attr.Value = value ?? "";
            current.Attributes!.Append(attr);
        }
        else throw new Exception("Current attribute is null!");
    }

    public override void WriteStartElement(string elementName)
    {
        var current = _nodeStack.Peek();
        var node = _xmlDocument.CreateElement(KbinConverter.GetRepairedName(elementName, _readOptions.RepairedPrefix));
        current.AppendChild(node);
        _nodeStack.Push(node);
    }

    public override void WriteEndElement()
    {
        _nodeStack.Pop();
    }

    public override void WriteStartAttribute(string value)
    {
        _holdAttrName = KbinConverter.GetRepairedName(value, _readOptions.RepairedPrefix);
    }

    public override void WriteEndAttribute()
    {
        _holdAttrName = null;
    }

    public override object GetResult()
    {
        return _xmlDocument;
    }
}