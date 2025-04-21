// ReSharper disable file StringLiteralTypo

using BotwRstb.Generator.Extensions;

namespace BotwRstb.Generator.ResourceTypes;

public class AampResource
{
    public static uint EstimateSize(uint fileSize, ReadOnlySpan<char> extension, Platform platform)
    {
        double size = fileSize * 1.05;
        size = extension switch {
            "bassetting" => size.Round32() + (size + 2.75) + platform switch {
                Platform.WiiU => 0x2BC,
                >= Platform.Switch => 0x3C8,
            },
            "bdmgparam" => (-0.0018 * size + 6.6273) * size + 500.0,
            "bphysics" => (size.Round32() + 0x4E + 0x324) * Math.Max(4.0 * double.Floor(size / 1388.0), 3.0),
            "baiprog" => size * size switch {
                <= 380 => 7.0,
                <= 400 => 6.0,
                <= 450 => 5.5,
                <= 600 => 5.0,
                <= 1000 => 4.0,
                <= 1750 => 3.5,
                _ => 3.0,
            },
            "bas" => size * 1.05 * size switch {
                <= 100 => 20.0,
                <= 200 => 12.5,
                <= 300 => 10.0,
                <= 600 => 8.0,
                <= 1500 => 6.0,
                <= 2000 => 5.5,
                <= 15000 => 5.0,
                _ => 4.5,
            },
            "baslist" => size * size switch {
                <= 100 => 15.0,
                <= 200 => 10.0,
                <= 300 => 8.0,
                <= 500 => 6.0,
                <= 800 => 5.0,
                <= 4000 => 4.0,
                _ => 3.5,
            },
            "bdrop" => size * size switch {
                <= 200 => 8.5,
                <= 250 => 7.0,
                <= 350 => 6.0,
                <= 450 => 5.25,
                <= 850 => 4.5,
                _ => 4.0,
            },
            "bgparamlist" => size * size switch {
                <= 100 => 20.0,
                <= 150 => 12.0,
                <= 250 => 10.0,
                <= 350 => 8.0,
                <= 450 => 7.0,
                _ => 6.0,
            },
            "brecipe" => size * size switch {
                <= 100 => 12.5,
                <= 160 => 8.5,
                <= 200 => 7.5,
                <= 215 => 7.0,
                _ => 6.5,
            },
            "bshop" => size * size switch {
                <= 200 => 7.25,
                <= 400 => 6.0,
                <= 500 => 5.0,
                _ => 4.05,
            },
            "bxml" => size * size switch {
                <= 350 => 6.0,
                <= 450 => 5.0,
                <= 550 => 4.5,
                <= 650 => 4.0,
                <= 800 => 3.5,
                _ => 3.0,
            },
            _ => throw new NotSupportedException(
                message: $"Unspported AMMP file format: '{extension}'")
        };

        return (uint)(size * platform switch {
            Platform.WiiU => 1,
            >= Platform.Switch => 1.5,
        });
    }
}