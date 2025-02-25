using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Xml.Linq;
using KbinXml.Net.Utils;

namespace KbinXml.Net.Internal.Loggers;

#if DEBUG
internal class ConsoleLogger : IKbinLogger
{
    [DebuggerStepThrough]
    public void LogSignature(byte signature, long position)
    {
        Console.WriteLine($"0x{position:X8}   Signature: 0x{signature:X2}");
    }

    [DebuggerStepThrough]
    public void LogCompression(byte compressionFlag, long position)
    {
        Console.WriteLine($"0x{position:X8}   Compression: 0x{compressionFlag:X2}");
    }

    [DebuggerStepThrough]
    public void LogEncoding(byte encodingFlag, long position)
    {
        Console.WriteLine($"0x{position:X8}   Encoding: 0x{encodingFlag:X2}");
    }

    [DebuggerStepThrough]
    public void LogEncodingNot(byte encodingFlagNot, long position)
    {
        Console.WriteLine($"0x{position:X8}   Encoding~: 0x{encodingFlagNot:X2}");
    }

    [DebuggerStepThrough]
    public void LogNodeControl(byte type, long position, bool arrayFlag)
    {
        var str = $"node(0x{position:X8})   NodeControlType: {(ControlType)type}(0x{type:X2})";
        if (arrayFlag) str += ", With array flag";
        Console.WriteLine(str);
    }

    [DebuggerStepThrough]
    public void LogNodeData(NodeType type, long position, bool arrayFlag)
    {
        var str = $"node(0x{position:X8})   NodeDataType: {type.Name} (Size={type.Size}, Count={type.Count})";
        if (arrayFlag) str += ", With array flag";
        Console.WriteLine(str);
    }

    [DebuggerStepThrough]
    public void LogStructElement(string elementName, long position)
    {
        Console.WriteLine($"node(0x{position:X8})   StructElement: \"{elementName}\"");
    }

    [DebuggerStepThrough]
    public void LogAttributeName(string attrName, long position)
    {
        Console.WriteLine($"node(0x{position:X8})   AttrName: \"{attrName}\"");
    }

    [DebuggerStepThrough]
    public void LogAttributeLength(int length, long position, string? flag)
    {
        Console.WriteLine($"{flag,4}(0x{position:X8})   AttrLen: \"{length}\"");
    }

    [DebuggerStepThrough]
    public void LogAttributeValue(string value, long position, string flag)
    {
        var arr = value.SelectMany(c => char.IsControl(c) ? DebugUtils.GetDisplayable(c) : c.ToString());
        var str1 = new string(arr.ToArray());
        Console.WriteLine($"{flag,4}(0x{position:X8}) o AttrValue: \"{str1}\"");
    }

    [DebuggerStepThrough]
    public void LogDataElement(string elementName, long position)
    {
        Console.WriteLine($"node(0x{position:X8})   DataElement: \"{elementName}\"");
    }

    [DebuggerStepThrough]
    public void LogArraySize(int size, long position, string? flag)
    {
        Console.WriteLine($"{flag,4}(0x{position:X8})   ArraySize: {size}");
    }

    [DebuggerStepThrough]
    public void LogStringValue(string value, long position, string flag)
    {
        Console.WriteLine($"{flag,4}(0x{position:X8}) o ValString: \"{value}\"");
    }

    [DebuggerStepThrough]
    public void LogBinaryValue(string value, long position, string flag)
    {
        Console.WriteLine($"{flag,4}(0x{position:X8}) o ValBinary: \"{value}\"");
    }

    [DebuggerStepThrough]
    public void LogArrayValue(string value, long position, string flag)
    {
        Console.WriteLine($"{flag,4}(0x{position:X8}) o ValArray: \"{value}\"");
    }

    [DebuggerStepThrough]
    public void LogNodeLength(int length, long position)
    {
        Console.WriteLine($"0x{position:X8}   NodeLength: {length}");
    }

    [DebuggerStepThrough]
    public void LogDataLength(int length, long position)
    {
        Console.WriteLine($"0x{position:X8}   DataLength: {length}");
    }

    [DebuggerStepThrough]
    public void LogXmlAttribute(string attributeName, string? value)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{"linq",4}{"",12}   Attribute: {attributeName}={value}");
        Console.ResetColor();
    }

    [DebuggerStepThrough]
    public void LogXmlNodeStart(Stack<XContainer> nodeStack)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{"linq",4}{"",12}   NodeStart: {GetNodePath(nodeStack)}");
        Console.ResetColor();
    }

    [DebuggerStepThrough]
    public void LogXmlNodeEnd(Stack<XContainer> nodeStack)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine($"{"linq",4}{"",12}   NodeEnd: {GetNodePath(nodeStack)}");
        Console.ResetColor();
    }

    private static string GetNodePath(Stack<XContainer> nodeStack)
    {
        var e = string.Join(".", nodeStack.ToArray().Reverse().Select(k =>
        {
            if (k == null) throw new ArgumentNullException(nameof(k));
            if (k is XElement xe) return xe.Name.ToString();
            if (k is XDocument xd) return null;
            throw new ArgumentOutOfRangeException();
        }).Where(k => k != null));
        return e;
    }
}
#endif