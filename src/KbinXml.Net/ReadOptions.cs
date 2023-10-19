namespace KbinXml.Net;

public class ReadOptions
{
    /// <summary>
    /// Set whether to fix invalid XML names by prefix (e.g.: Node names start with numbers).
    /// Set to null to disable this function.
    /// The default value is null.
    /// </summary>
    public string? RepairedPrefix { get; set; }
}