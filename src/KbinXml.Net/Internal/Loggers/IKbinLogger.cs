#if DEBUG
using System.Collections.Generic;
using System.Xml.Linq;
#endif

namespace KbinXml.Net.Internal.Loggers;

internal interface IKbinLogger
{
#if DEBUG
    void LogSignature(byte signature, long position);
    void LogCompression(byte compressionFlag, long position);
    void LogEncoding(byte encodingFlag, long position);
    void LogEncodingNot(byte encodingFlagNot, long position);
    void LogNodeControl(byte type, long position, bool arrayFlag);
    void LogNodeData(NodeType type, long position, bool arrayFlag);
    void LogStructElement(string elementName, long position);
    void LogAttributeName(string attrName, long position);
    void LogAttributeLength(int length, long position, string flag);
    void LogAttributeValue(string value, long position, string flag);
    void LogDataElement(string elementName, long position);
    void LogArraySize(int size, long position, string flag);
    void LogStringValue(string value, long position, string flag);
    void LogBinaryValue(string value, long position, string flag);
    void LogArrayValue(string value, long position, string flag);
    void LogNodeLength(int length, long position);
    void LogDataLength(int length, long position);
    void LogXmlAttribute(string attributeName, string? value);
    void LogXmlNodeStart(Stack<XContainer> nodeStack);
    void LogXmlNodeEnd(Stack<XContainer> nodeStack);
#endif
}
