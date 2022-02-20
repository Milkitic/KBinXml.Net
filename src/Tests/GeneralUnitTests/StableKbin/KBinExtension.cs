using System;
using System.IO;
using System.Text;
using System.Xml.Linq;

namespace StableKbin
{
    public static class KBinExtension
    {
        public static string ToStringWithDeclaration(this XDocument xDocument, SaveOptions options = SaveOptions.DisableFormatting)
        {
            if (options == SaveOptions.DisableFormatting)
            {
                return xDocument.Declaration + xDocument.ToString(options);
            }

            return xDocument.Declaration + Environment.NewLine + xDocument.ToString(options);
        }

        public static byte[] ToArray(this Stream stream)
        {
            if (stream is MemoryStream ms1)
                return ms1.ToArray();

            var pos = stream.Position;
            stream.Position = 0;
            byte[] buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }

                stream.Position = pos;
                return ms.ToArray();
            }
        }

        public static Encoding ToEncoding(this KBinEncodings kBinEncoding)
        {
            Encoding encoding = null;
            switch (kBinEncoding)
            {
                case KBinEncodings.ShiftJIS:
                    encoding = Encoding.GetEncoding("shift_jis");
                    break;
                case KBinEncodings.ASCII:
                    encoding = Encoding.ASCII;
                    break;
                case KBinEncodings.ISO_8859_1:
                    encoding = Encoding.GetEncoding("iso-8859-1");
                    break;
                case KBinEncodings.EUCJP:
                    encoding = Encoding.GetEncoding("EUC-JP");
                    break;
                case KBinEncodings.UTF8:
                    encoding = Encoding.UTF8;
                    break;
                default:
                    break;
            }

            return encoding;
        }
    }
}
