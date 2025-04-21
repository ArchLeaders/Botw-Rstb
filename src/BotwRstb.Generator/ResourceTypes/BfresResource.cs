using System.Runtime.CompilerServices;

namespace BotwRstb.Generator.ResourceTypes;

public class BfresResource
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint EstimateSize(uint size, Platform platform)
    {
        return (uint)(size * platform switch {
            Platform.WiiU => size switch {
                <= 500 => 7.0,
                <= 750 => 5.0,
                <= 1250 => 4.0,
                <= 2000 => 3.5,
                <= 400000 => 2.25,
                <= 600000 => 2.1,
                <= 1000000 => 1.95,
                <= 1500000 => 1.85,
                <= 3000000 => 1.66,
                _ => 1.45,
            },
            >= Platform.Switch => size switch {
                <= 1_250 => 9.5,
                <= 2_500 => 6.0,
                <= 50_000 => 4.25,
                <= 100_000 => 3.66,
                <= 800_000 => 3.5,
                <= 2_000_000 => 3.15,
                <= 3_000_000 => 2.5,
                <= 4_000_000 => 1.667,
                _ => 1.6,
            }
        });
    }
}
