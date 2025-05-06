using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net.Internal;
using KbinXml.Net.Internal.Writers;
using KbinXml.Net.Utils;
using U8Xml;

namespace KbinXml.Net;

public static partial class KbinConverter
{
    public static byte[] WriteUtf8(string xmlText, WriteOptions? writeOptions = null)
    {
        if (string.IsNullOrEmpty(xmlText))
        {
            throw new ArgumentNullException(nameof(xmlText));
        }

        var encoding = KnownEncodings.UTF8;
        writeOptions ??= new WriteOptions();
        using (XmlObject xml = XmlParser.Parse(xmlText))
        {

        }

        return Array.Empty<byte>();
    }

    public static byte[] WriteUtf8(ReadOnlySpan<byte> xmlUtf8Text, WriteOptions? writeOptions = null)
    {
        if (xmlUtf8Text.IsEmpty)
        {
            return Array.Empty<byte>();
        }

        var encoding = KnownEncodings.UTF8;
        writeOptions ??= new WriteOptions();
        using (XmlObject xml = XmlParser.Parse(xmlUtf8Text))
        {

        }

        return Array.Empty<byte>();
    }
}