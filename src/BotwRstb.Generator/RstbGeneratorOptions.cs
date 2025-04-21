using RstbLibrary;

namespace BotwRstb.Generator;

public enum Platform : byte
{
    WiiU,
    Switch
}

public record RstbGeneratorOptions(
    (string Path, bool IsAoc)[] InputContentFolders,
    string SourceRstbFile,
    Platform Platform,
    string? OutputRstbFile = null,
    uint Padding = 0
);
