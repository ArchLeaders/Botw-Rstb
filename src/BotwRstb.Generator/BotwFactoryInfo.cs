#pragma warning disable CS8524 // The switch expression does not handle some values of its input type (it is not exhaustive) involving an unnamed enum value.
// ReSharper disable file StringLiteralTypo

namespace BotwRstb.Generator;

public enum ParseMode
{
    Simple,
    Complex
}

public readonly ref struct BotwFactoryInfo(uint size, uint alignment, ParseMode parseMode, uint simpleParseSize = 0)
{
    public readonly uint Size = size;
    public readonly uint Alignment = alignment;
    public readonly uint SimpleParseSize = simpleParseSize;
    public readonly ParseMode ParseMode = parseMode;

    public static implicit operator BotwFactoryInfo((uint, uint, uint) info)
        => new BotwFactoryInfo(info.Item1, info.Item2, ParseMode.Simple, info.Item3);

    public static implicit operator BotwFactoryInfo((uint, uint) info)
        => new BotwFactoryInfo(info.Item1, info.Item2, ParseMode.Complex);
    
    public static BotwFactoryInfo Get(ReadOnlySpan<char> canonical, ReadOnlySpan<char> ext, Platform platform)
    {
        return ext switch {
            "sarc" or "pack" or "bactorpack" or "bmodelsh" or "beventpack" or "stera" or "stats" => platform switch {
                Platform.Switch => (0x68, 0x80, 0),
                Platform.WiiU => (0x3c, 0x80, 0),
            },
            "bfres" when canonical.Length > 4 && canonical[..^4] is ".Tex" or "Tex1" or "Tex2" => platform switch {
                Platform.Switch => (0x38, 0x8, 0),
                Platform.WiiU => (0x20, 0x4, 0),
            },
            "bfres" => platform switch {
                Platform.Switch => (0x1a8, 0x1000),
                Platform.WiiU => (0x13c, 0x1000),
            },
            "bcamanim" => platform switch {
                Platform.Switch => (0x50, 0x2000),
                Platform.WiiU => (0x2c, 0x2000),
            },
            "batpl" or "bnfprl" => platform switch {
                Platform.Switch => (0x40, 0x8, 0),
                Platform.WiiU => (0x24, 0x4, 0),
            },
            "bplacement" => platform switch {
                Platform.Switch => (0x48, 0x8, 0),
                Platform.WiiU => (0x14, 0x4, 0),
            },
            "hks" or "lua" => platform switch {
                Platform.Switch => (0x38, 0x8, 0),
                Platform.WiiU => (0x14, 0x4, 0),
            },
            "bactcapt" => platform switch {
                Platform.Switch => (0x538, 0x8, 0),
                Platform.WiiU => (0x3b4, 0x4, 0),
            },
            "bitemico" => platform switch {
                Platform.Switch => (0x60, 0x2000, 0),
                Platform.WiiU => (0xd0, 0x2000, 0),
            },
            "jpg" => platform switch {
                Platform.Switch => (0x80, 0x2000, 0),
                Platform.WiiU => (0x174, 0x2000, 0),
            },
            "bmaptex" => platform switch {
                Platform.Switch => (0x60, 0x2000, 0),
                Platform.WiiU => (0xd0, 0x2000, 0),
            },
            "bstftex" or "bmapopen" or "breviewtex" => platform switch {
                Platform.Switch => (0x60, 0x2000, 0),
                Platform.WiiU => (0xd0, 0x2000, 0),
            },
            "bgdata" => platform switch {
                Platform.Switch => (0x140, 0x8, 0),
                Platform.WiiU => (0xcc, 0x4, 0),
            },
            "bgsvdata" => platform switch {
                Platform.Switch => (0x38, 0x8, 0),
                Platform.WiiU => (0x14, 0x4, 0),
            },
            "hknm2" => platform switch {
                Platform.Switch => (0x48, 0x8),
                Platform.WiiU => (0x28, 0x4),
            },
            "bmscdef" => platform switch {
                Platform.Switch => (0x2a8, 0x8),
                Platform.WiiU => (0x1fc, 0x4),
            },
            "bars" => platform switch {
                Platform.Switch => (0xb0, 0x80),
                Platform.WiiU => (0x84, 0x80),
            },
            "bxml" => platform switch {
                Platform.Switch => (0x778, 0x8),
                Platform.WiiU => (0x4a8, 0x4),
            },
            "bgparamlist" => platform switch {
                Platform.Switch => (0x2c0, 0x8),
                Platform.WiiU => (0x248, 0x4),
            },
            "bmodellist" => platform switch {
                Platform.Switch => (0x7d0, 0x8),
                Platform.WiiU => (0x508, 0x4),
            },
            "baslist" => platform switch {
                Platform.Switch => (0x410, 0x8),
                Platform.WiiU => (0x2f4, 0x4),
            },
            "baiprog" => platform switch {
                Platform.Switch => (0x448, 0x8),
                Platform.WiiU => (0x30c, 0x4),
            },
            "bphysics" => platform switch {
                Platform.Switch => (0x470, 0x8),
                Platform.WiiU => (0x324, 0x4),
            },
            "bchemical" => platform switch {
                Platform.Switch => (0x3c0, 0x8),
                Platform.WiiU => (0x2cc, 0x4),
            },
            "bas" => platform switch {
                Platform.Switch => (0x3c8, 0x8),
                Platform.WiiU => (0x2d0, 0x4),
            },
            "batcllist" => platform switch {
                Platform.Switch => (0x3f0, 0x8),
                Platform.WiiU => (0x2e4, 0x4),
            },
            "batcl" => platform switch {
                Platform.Switch => (0x428, 0x8),
                Platform.WiiU => (0x344, 0x4),
            },
            "baischedule" => platform switch {
                Platform.Switch => (0x2b8, 0x8, 0x20),
                Platform.WiiU => (0x244, 0x4, 0x18),
            },
            "bdmgparam" => platform switch {
                Platform.Switch => (0x11d0, 0x8, 0x7f8),
                Platform.WiiU => (0x9f0, 0x4, 0x40c),
            },
            "brgconfiglist" => platform switch {
                Platform.Switch => (0x3d0, 0x8),
                Platform.WiiU => (0x2d4, 0x4),
            },
            "brgconfig" => platform switch {
                Platform.Switch => (0x42d8, 0x8, 0x20),
                Platform.WiiU => (0x2acc, 0x4, 0x10),
            },
            "brgbw" => platform switch {
                Platform.Switch => (0x2c0, 0x8),
                Platform.WiiU => (0x248, 0x4),
            },
            "bawareness" => platform switch {
                Platform.Switch => (0xb38, 0x8, 0x20),
                Platform.WiiU => (0x70c, 0x4, 0x10),
            },
            "bdrop" => platform switch {
                Platform.Switch => (0x320, 0x8),
                Platform.WiiU => (0x27c, 0x4),
            },
            "bshop" => platform switch {
                Platform.Switch => (0x320, 0x8),
                Platform.WiiU => (0x27c, 0x4),
            },
            "brecipe" => platform switch {
                Platform.Switch => (0x320, 0x8),
                Platform.WiiU => (0x27c, 0x4),
            },
            "blod" => platform switch {
                Platform.Switch => (0x3c0, 0x8, 0x18),
                Platform.WiiU => (0x2cc, 0x4, 0x10),
            },
            "bbonectrl" => platform switch {
                Platform.Switch => (0x8d0, 0x8),
                Platform.WiiU => (0x564, 0x4),
            },
            "blifecondition" => platform switch {
                Platform.Switch => (0x4b0, 0x8),
                Platform.WiiU => (0x35c, 0x4),
            },
            "bumii" => platform switch {
                Platform.Switch => (0x2b8, 0x8, 0x20),
                Platform.WiiU => (0x244, 0x4, 0x18),
            },
            "baniminfo" => platform switch {
                Platform.Switch => (0x2c8, 0x8),
                Platform.WiiU => (0x24c, 0x4),
            },
            "byaml" => platform switch {
                Platform.Switch => (0x20, 0x8, 0),
                Platform.WiiU => (0x14, 0x4, 0),
            },
            "byml" => platform switch {
                Platform.Switch => (0x38, 0x8, 0x28),
                Platform.WiiU => (0x20, 0x4, 0x3c),
            },
            "bassetting" => platform switch {
                Platform.Switch => (0x260, 0x8),
                Platform.WiiU => (0x1d8, 0x4),
            },
            "hkrb" => platform switch {
                Platform.Switch => (0x20, 0x8, 0x18),
                Platform.WiiU => (0x14, 0x4, 0x8),
            },
            "hkrg" => platform switch {
                Platform.Switch => (0x20, 0x8, 0x18),
                Platform.WiiU => (0x14, 0x4, 0x8),
            },
            "bphyssb" => platform switch {
                Platform.Switch => (0x5b0, 0x8),
                Platform.WiiU => (0x384, 0x4),
            },
            "hkcl" => platform switch {
                Platform.Switch => (0xe8, 0x8),
                Platform.WiiU => (0xb8, 0x4),
            },
            "hksc" => platform switch {
                Platform.Switch => (0x140, 0x8),
                Platform.WiiU => (0xe8, 0x4),
            },
            "hktmrb" => platform switch {
                Platform.Switch => (0x48, 0x8),
                Platform.WiiU => (0x28, 0x4),
            },
            "brgcon" => platform switch {
                Platform.Switch => (0x48, 0x8),
                Platform.WiiU => (0x28, 0x4),
            },
            "esetlist" => platform switch {
                Platform.Switch => (0x38, 0x4000, 0),
                Platform.WiiU => (0x20, 0x4000, 0),
            },
            "bdemo" => platform switch {
                Platform.Switch => (0xb20, 0x8, 0x18),
                Platform.WiiU => (0x6cc, 0x4, 0x10),
            },
            "bfevfl" => platform switch {
                Platform.Switch => (0x40, 0x8, 0x18),
                Platform.WiiU => (0x24, 0x4, 0x38),
            },
            "bfevtm" => platform switch {
                Platform.Switch => (0x40, 0x8, 0x18),
                Platform.WiiU => (0x24, 0x4, 0x38),
            },
            _ => platform switch {
                Platform.Switch => (0x38, 0x8, 0),
                Platform.WiiU => (0x20, 0x4, 0),
            },
        };
    }
}