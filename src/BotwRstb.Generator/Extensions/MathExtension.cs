namespace BotwRstb.Generator.Extensions;

public static class MathExtension
{
    public static uint Round32(this uint value)
    {
        return (uint)((value + 31) & -32);
    }
    
    public static uint Round64(this uint value)
    {
        return (uint)((value + 63) & -64);
    }

    public static double Round32(this double value)
    {
        return ((uint)value + 31) & -32;
    }
}
