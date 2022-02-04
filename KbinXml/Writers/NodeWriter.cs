using System;
using System.Collections.Generic;
using System.Text;
using KbinXml.Utils;

namespace KbinXml.Writers
{
    public class NodeWriter : BeBinaryWriter
    {
        private readonly bool _compressed;
        private readonly Encoding _encoding;

        public NodeWriter(bool compressed, Encoding encoding)
        {
            _compressed = compressed;
            _encoding = encoding;
        }

        public void WriteString(string value)
        {
            if (_compressed)
            {
                WriteU8((byte)value.Length);
                SixbitHelper.EncodeAndWrite(Stream, value);
            }
            else
            {
                WriteU8((byte)((value.Length - 1) | (1 << 6)));
                WriteBytes(_encoding.GetBytes(value));
            }
        }
    }
}
