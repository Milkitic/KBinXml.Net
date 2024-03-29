﻿namespace KbinXml.Net;

public class WriteOptions
{
    /// <summary>
    /// Set whether to be strict mode that disabling to fix the array length matching the "__count" attribute.
    /// The default value is true.
    /// </summary>
    public bool StrictMode { get; set; } = true;

    /// <summary>
    /// Set whether to compress XML data.
    /// The default value is true.
    /// </summary>
    public bool Compress { get; set; } = true;

    /// <summary>
    /// Set whether to repair invalid XML names by prefix (e.g.: Node names start with numbers).
    /// Set to null to disable this function.
    /// The default value is null.
    /// </summary>
    public string? RepairedPrefix { get; set; }
}