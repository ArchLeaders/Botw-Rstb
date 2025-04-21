namespace BotwRstb.Generator.Extensions;

public static class RomfsExtension
{
    private const char NULLCHAR = '\0';
    private const string AOC_PREFIX = "Aoc/0010/";

    public static string ToCanonical(this string path, string romfs, bool isAoc, out ReadOnlySpan<char> extension)
    {
        if (romfs.Length > path.Length) {
            throw new ArgumentException(
                message: "The romfs path cannot be larger than the input path.",
                nameof(romfs)
            );
        }

        int romfsLength = romfs.Length;
        if (romfs[^1] is not ('\\' or '/')) {
            romfsLength++;
        }

        return ToCanonical(path.AsSpan()[romfsLength..], isAoc, out extension);
    }

    public static string ToCanonical(this string relativePath, bool isAoc, out ReadOnlySpan<char> extension)
        => ToCanonical(relativePath.AsSpan(), isAoc, out extension);
    
    public static unsafe string ToCanonical(this ReadOnlySpan<char> relativePath, bool isAoc, out ReadOnlySpan<char> extension)
    {
        extension = Path.GetExtension(relativePath)[1..];

        int resultLength = isAoc switch {
            true => relativePath.Length + AOC_PREFIX.Length,
            false => relativePath.Length
        };

        bool removeSyazPrefix = extension.Length > 0 && extension[0] is 's' &&
                                extension is not "sarc" or "stera" or "stats";

        if (removeSyazPrefix) {
            resultLength--;
            extension = extension[1..];
        }

        string result = new(NULLCHAR, resultLength);

        Span<char> mutable;
        fixed (char* ptr = result) {
            mutable = new Span<char>(ptr, result.Length);
        }

        int writeIndex = 0;
        if (isAoc) {
            writeIndex = AOC_PREFIX.Length;
            for (int i = 0; i < writeIndex; i++) {
                mutable[i] = AOC_PREFIX[i];
            }
        }

        char lastChar = NULLCHAR;
        foreach (char @char in relativePath) {
            char charToWrite = (@char, removeSyazPrefix, lastChar) switch {
                ('\\', _, _) => '/',
                ('s', true, '.') => NULLCHAR,
                _ => @char
            };

            if (charToWrite is not NULLCHAR) {
                mutable[writeIndex] = charToWrite;
                writeIndex++;
            }

            lastChar = @char;
        }

        return result;
    }
}
