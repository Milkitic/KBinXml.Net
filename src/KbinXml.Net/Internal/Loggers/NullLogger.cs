using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

namespace KbinXml.Net.Internal.Loggers;

#if !DEBUG
internal class NullLogger : IKbinLogger
{
    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogSignature(byte signature, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogCompression(byte compressionFlag, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogEncoding(byte encodingFlag, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogEncodingNot(byte encodingFlagNot, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogNodeControl(byte type, long position, bool arrayFlag)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogNodeData(NodeType type, long position, bool arrayFlag)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogStructElement(string elementName, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogAttributeName(string attrName, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogAttributeLength(int length, long position, string flag)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogAttributeValue(string value, long position, string flag)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogDataElement(string elementName, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogArraySize(int size, long position, string flag)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogStringValue(string value, long position, string flag)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogBinaryValue(string value, long position, string flag)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogArrayValue(string value, long position, string flag)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogNodeLength(int length, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogDataLength(int length, long position)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogXmlAttribute(string attributeName, string? value)
    {
    }

    [Conditional("DEBUG")]
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void LogXmlNodeStart(Stack<XContainer> nodeStack)
    {
    }

    [Conditional("DEBUG")]
    [InlineMethod.Inline]
    public void LogXmlNodeEnd(Stack<XContainer> nodeStack)
    {
    }
}
#endif