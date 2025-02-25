namespace KbinXml.Net;

/// <summary>
/// Represents configuration options for KBin reading operations.
/// </summary>
public class ReadOptions
{
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