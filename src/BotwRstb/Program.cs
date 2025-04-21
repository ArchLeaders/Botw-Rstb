using BotwRstb.Generator;

RstbGenerator generator = new(new RstbGeneratorOptions(
    InputContentFolders: [
        (@"D:\Mods\BotW\Archive\The-Heros-Bunker\Merged\Build\content", IsAoc: false),
        (@"D:\Mods\BotW\Archive\The-Heros-Bunker\Merged\Build\aoc\0010\", IsAoc: true)
    ],
    SourceRstbFile: @"D:\Games\Emulation\RomFS\Botw\1.5.0\content\System\Resource\ResourceSizeTable.product.srsizetable",
    Platform.WiiU,
    OutputRstbFile: "D:\\bin\\BotwRstb\\ResourceSizeTable.product.srsizetable"
));

generator.Generate();
