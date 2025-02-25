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
}