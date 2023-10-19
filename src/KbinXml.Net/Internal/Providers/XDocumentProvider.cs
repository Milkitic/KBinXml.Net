using System;
using System.Collections.Generic;
using System.Linq;
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
#if DEBUG
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine($"{"linq",4}{"",12}   Attribute: {_holdAttrName}={value}");
            Console.ResetColor();
#endif
        }
        else throw new Exception("Current attribute is null!");
    }

    public override void WriteStartElement(string elementName)
    {
        var current = _nodeStack.Peek();

        var node = new XElement(KbinConverter.GetRepairedName(elementName, _readOptions.RepairedPrefix));
        current.Add(node);
        _nodeStack.Push(node);
#if DEBUG
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{"linq",4}{"",12}   NodeStart: {GetNodePath()}");
        Console.ResetColor();
#endif
    }

    public override void WriteEndElement()
    {
#if DEBUG
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{"linq",4}{"",12}   NodeEnd: {GetNodePath()}");
        Console.ResetColor();
#endif
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
        return _xDocument;
    }

    private string GetNodePath()
    {
        var e = string.Join(".", _nodeStack.ToArray().Reverse().Select(k =>
        {
            if (k == null) throw new ArgumentNullException(nameof(k));
            if (k is XElement xe)
            {
                return xe.Name.ToString();
            }
            else if (k is XDocument xd)
            {
                return null;
            }

            throw new ArgumentOutOfRangeException();
        }).Where(k => k != null));
        return e;
    }
}