namespace KbinXml.Net;

/// <summary>
/// Represents configuration options for KBin writing operations.
/// </summary>
public class WriteOptions
{
    /// <summary>
    /// Gets or sets a value indicating whether strict validation is enforced during serialization.
    /// When <see langword="true"/>, mismatches between array lengths and '__count' attributes will throw exceptions.
    /// </summary>
    /// <value>
    /// The default value is <see langword="true"/> (strict validation enabled).
    /// </value>
    public bool StrictMode { get; set; } = true;

    /// <summary>
    /// Gets or sets a value indicating whether XML output should be compressed using SixBit algorithm.
    /// </summary>
    /// <value>
    /// The default value is <see langword="true"/> (compression enabled).
    /// </value>
    public bool Compress { get; set; } = true;

    /// <summary>
    /// Gets or sets the prefix used to repair invalid XML element names during serialization.  (e.g.: Names which start with numbers).
    /// When set to a non-null value, invalid names will be prefixed with this string.
    /// Set to <see langword="null"/> to disable automatic name repair.
    /// </summary>
    /// <value>
    /// The default value is <see langword="null"/> (repair disabled).
    /// </value>
    public string? RepairedPrefix { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the writer should fill alignment gaps with zeros.
    /// </summary>
    /// <remarks>
    /// The kbin format requires 32-bit data (like binary blobs) to be 4-byte aligned. When the writer
    /// inserts 8-bit or 16-bit values, it may need to add 1-3 padding bytes (a "gap") before the next
    /// 32-bit data block to ensure this alignment.
    /// <para>
    /// Setting this to <see langword="true"/> ensures these gaps are filled with <c>0x00</c>,
    /// which matches the specification and is safer.
    /// </para>
    /// <para>
    /// Setting this to <see langword="false"/> skips the zero-filling step. This can improve write
    /// performance but may leave "dirty" data (from recycled memory buffers) in the gaps. This is
    /// usually safe if the parser correctly skips gaps based on data offsets and lengths, rather than
    /// reading them.
    /// </para>
    /// </remarks>
    /// <value>
    /// The default value is <see langword="true"/> (gaps are filled with zeros).
    /// </value>
    public bool ZeroFillGap { get; set; } = true;
}