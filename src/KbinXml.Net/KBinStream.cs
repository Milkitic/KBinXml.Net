using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using KbinXml.Net.Utils;
using Microsoft.IO;

namespace KbinXml.Net
{
    /// <summary>
    /// 提供高性能的KBin格式编解码流操作。
    /// 可以直接创建新的流或包装现有流进行KBin格式的编解码。
    /// </summary>
    internal class KBinStream : Stream
    {
        private readonly Stream _baseStream;
        private readonly bool _ownsStream;
        private bool _disposed;

        /// <summary>
        /// 获取或设置当前流的位置。
        /// </summary>
        public override long Position
        {
            get => _baseStream.Position;
            set => _baseStream.Position = value;
        }

        /// <summary>
        /// 获取流的长度。
        /// </summary>
        public override long Length => _baseStream.Length;

        /// <summary>
        /// 获取一个值，该值指示流是否支持读取。
        /// </summary>
        public override bool CanRead => _baseStream.CanRead;

        /// <summary>
        /// 获取一个值，该值指示流是否支持写入。
        /// </summary>
        public override bool CanWrite => _baseStream.CanWrite;

        /// <summary>
        /// 获取一个值，该值指示流是否支持查找。
        /// </summary>
        public override bool CanSeek => _baseStream.CanSeek;

        /// <summary>
        /// 创建一个新的KBinStream实例。
        /// </summary>
        public KBinStream()
            : this(KbinConverter.RecyclableMemoryStreamManager.GetStream("kbs"), true)
        {
        }

        /// <summary>
        /// 使用指定的初始容量创建一个新的KBinStream实例。
        /// </summary>
        /// <param name="capacity">初始容量。</param>
        public KBinStream(int capacity)
            : this(KbinConverter.RecyclableMemoryStreamManager.GetStream("kbs", capacity), true)
        {
        }

        /// <summary>
        /// 使用指定的流创建一个新的KBinStream实例。
        /// </summary>
        /// <param name="stream">要包装的流。</param>
        /// <param name="ownsStream">指示是否在释放KBinStream时释放基础流。</param>
        public KBinStream(Stream stream, bool ownsStream = false)
        {
            _baseStream = stream ?? throw new ArgumentNullException(nameof(stream));
            _ownsStream = ownsStream;
        }

        /// <summary>
        /// 从流中读取字节块。
        /// </summary>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">buffer中的偏移量。</param>
        /// <param name="count">要读取的最大字节数。</param>
        /// <returns>读取的字节数。</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _baseStream.Read(buffer, offset, count);
        }

        /// <summary>
        /// 将字节块写入流。
        /// </summary>
        /// <param name="buffer">字节数组。</param>
        /// <param name="offset">buffer中的偏移量。</param>
        /// <param name="count">要写入的字节数。</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            _baseStream.Write(buffer, offset, count);
        }

        /// <summary>
        /// 设置流中的位置。
        /// </summary>
        /// <param name="offset">相对于origin的字节偏移量。</param>
        /// <param name="origin">指示用于获取新位置的参考点。</param>
        /// <returns>流中的新位置。</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _baseStream.Seek(offset, origin);
        }

        /// <summary>
        /// 设置流的长度。
        /// </summary>
        /// <param name="value">所需的流长度。</param>
        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        /// <summary>
        /// 清除流的所有缓冲区，并使所有缓冲数据写入基础设备。
        /// </summary>
        public override void Flush()
        {
            _baseStream.Flush();
        }

        /// <summary>
        /// 将流的内容转换为字节数组。
        /// </summary>
        /// <returns>包含流内容的字节数组。</returns>
        public byte[] ToArray()
        {
            if (_baseStream is MemoryStream ms)
            {
                return ms.ToArray();
            }
            else if (_baseStream is RecyclableMemoryStream rms)
            {
                return rms.ToArray();
            }
            else
            {
                return _baseStream.ToArray();
            }
        }

        #region KBin编解码方法

        /// <summary>
        /// 将KBin二进制数据解码为XDocument。
        /// </summary>
        /// <param name="readOptions">读取选项。</param>
        /// <returns>解析后的XML文档。</returns>
        public XDocument Decode(ReadOptions? readOptions = null)
        {
            var buffer = ToArray();
            return DecodeBuffer(buffer, readOptions);
        }

        /// <summary>
        /// 将KBin二进制数据解码为XDocument，并输出检测到的编码。
        /// </summary>
        /// <param name="knownEncodings">检测到的编码。</param>
        /// <param name="readOptions">读取选项。</param>
        /// <returns>解析后的XML文档。</returns>
        public XDocument Decode(out KnownEncodings knownEncodings, ReadOptions? readOptions = null)
        {
            var buffer = ToArray();
            return DecodeBuffer(buffer, out knownEncodings, readOptions);
        }

        /// <summary>
        /// 将KBin二进制数据解码为XmlDocument。
        /// </summary>
        /// <param name="readOptions">读取选项。</param>
        /// <returns>解析后的XML文档。</returns>
        public XmlDocument DecodeToXmlDocument(ReadOptions? readOptions = null)
        {
            var buffer = ToArray();
            return DecodeBufferToXmlDocument(buffer, readOptions);
        }

        /// <summary>
        /// 将KBin二进制数据解码为XmlDocument，并输出检测到的编码。
        /// </summary>
        /// <param name="knownEncodings">检测到的编码。</param>
        /// <param name="readOptions">读取选项。</param>
        /// <returns>解析后的XML文档。</returns>
        public XmlDocument DecodeToXmlDocument(out KnownEncodings knownEncodings, ReadOptions? readOptions = null)
        {
            var buffer = ToArray();
            return DecodeBufferToXmlDocument(buffer, out knownEncodings, readOptions);
        }

        /// <summary>
        /// 将XML文档编码为KBin二进制格式并写入流。
        /// </summary>
        /// <param name="xml">要编码的XML文档。</param>
        /// <param name="knownEncodings">要使用的编码。</param>
        /// <param name="writeOptions">写入选项。</param>
        public void Encode(XmlDocument xml, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
        {
            SetLength(0);
            Position = 0;
            KbinConverter.Write(xml, _baseStream, knownEncodings, writeOptions);
            Position = 0;
        }

        /// <summary>
        /// 将XML文档编码为KBin二进制格式并写入流。
        /// </summary>
        /// <param name="xml">要编码的XML文档。</param>
        /// <param name="knownEncodings">要使用的编码。</param>
        /// <param name="writeOptions">写入选项。</param>
        public void Encode(XDocument xml, KnownEncodings knownEncodings, WriteOptions? writeOptions = null)
        {
            SetLength(0);
            Position = 0;
            KbinConverter.Write(xml, _baseStream, knownEncodings, writeOptions);
            Position = 0;
        }

        #endregion

        #region 私有辅助方法

        private static XDocument DecodeBuffer(byte[] buffer, ReadOptions? readOptions = null)
        {
            return KbinConverter.ReadXmlLinq(buffer.AsSpan(), readOptions);
        }

        private static XDocument DecodeBuffer(byte[] buffer, out KnownEncodings knownEncodings, ReadOptions? readOptions = null)
        {
            return KbinConverter.ReadXmlLinq(buffer.AsSpan(), out knownEncodings, readOptions);
        }

        private static XmlDocument DecodeBufferToXmlDocument(byte[] buffer, ReadOptions? readOptions = null)
        {
            return KbinConverter.ReadXml(buffer.AsSpan(), readOptions);
        }

        private static XmlDocument DecodeBufferToXmlDocument(byte[] buffer, out KnownEncodings knownEncodings, ReadOptions? readOptions = null)
        {
            return KbinConverter.ReadXml(buffer.AsSpan(), out knownEncodings, readOptions);
        }

        #endregion

        #region IDisposable

        /// <summary>
        /// 释放由KBinStream使用的所有资源。
        /// </summary>
        /// <param name="disposing">指示是否释放托管资源。</param>
        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing && _ownsStream)
                {
                    _baseStream.Dispose();
                }

                _disposed = true;
            }

            base.Dispose(disposing);
        }

        #endregion
    }
}