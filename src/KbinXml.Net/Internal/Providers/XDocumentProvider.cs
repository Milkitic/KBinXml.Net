using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using System.Xml.Linq;

namespace KbinXml.Net.Internal.Providers;

internal class XDocumentProvider : WriterProvider
{
    private readonly ReadOptions _readOptions;
    private readonly XDocument _xDocument;
    private readonly Stack<XContainer> _nodeStack = new();

    private string? _holdAttrName;

    public XDocumentProvider(Encoding encoding, ReadOptions readOptions) : base(encoding)
    {
        _readOptions = readOptions;
        _xDocument = new XDocument(new XDeclaration("1.0", encoding.WebName, null));
        _nodeStack.Push(_xDocument);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteStartDocument()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteElementValue(string? value)
    {
        var current = _nodeStack.Peek();
        current.Add(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteAttributeValue(string? value)
    {
        if (_holdAttrName != null)
        {
            var current = _nodeStack.Peek();
            var attr = new XAttribute(XName.Get(_holdAttrName), value ?? "");
            current.Add(attr);
            KbinConverter.Logger.LogXmlAttribute(_holdAttrName, value);
        }
        else throw new Exception("Current attribute is null!");
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteStartElement(string elementName)
    {
        var current = _nodeStack.Peek();

        var node = new XElement(KbinConverter.GetRepairedName(elementName, _readOptions.RepairedPrefix));
        current.Add(node);
        _nodeStack.Push(node);
        KbinConverter.Logger.LogXmlNodeStart(_nodeStack);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteEndElement()
    {
        KbinConverter.Logger.LogXmlNodeEnd(_nodeStack);
        _nodeStack.Pop();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteStartAttribute(string value)
    {
        _holdAttrName = KbinConverter.GetRepairedName(value, _readOptions.RepairedPrefix);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override void WriteEndAttribute()
    {
        _holdAttrName = null;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override object GetResult()
    {
        return _xDocument;
    }
}