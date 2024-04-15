using RstbLibrary;

namespace BotwRstb.Generator;

public record RstbGeneratorOptions(string InputRomfs, string InputFile, string? OutputFile = null, uint Padding = 0);
