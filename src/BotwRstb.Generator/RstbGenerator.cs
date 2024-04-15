using CommunityToolkit.HighPerformance.Buffers;
using CsYaz0;
using CsYaz0.Marshalling;
using RstbLibrary;

namespace BotwRstb.Generator;

public class RstbGenerator
{
    private readonly RstbGeneratorOptions _options;
    private readonly Rstb _vanilla;
    private readonly Rstb _result;

    public RstbGenerator(RstbGeneratorOptions options)
    {
        _options = options;

        using FileStream fs = File.OpenRead(options.InputFile);
        int size = Convert.ToInt32(fs.Length);

        using SpanOwner<byte> buffer = SpanOwner<byte>.Allocate(size);
        fs.Read(buffer.Span);

        int decompressedSize = Yaz0.GetDecompressedSize(buffer.Span);
        using SpanOwner<byte> decompressed = SpanOwner<byte>.Allocate(decompressedSize);
        Yaz0.Decompress(buffer.Span, decompressed.Span);

        _vanilla = Rstb.FromBinary(decompressed.Span);
        _result = Rstb.FromBinary(decompressed.Span);
    }

    public async Task GenerateAsync()
    {
        await GenerateAsync(_options.InputRomfs);

        if (_options.OutputFile is not string output) {
            output = _options.InputFile;

            if (Path.GetDirectoryName(output) is string outputFolder) {
                Directory.CreateDirectory(outputFolder);
            }
        }

        byte[] result = _result.ToBinary();
        using DataMarshal compressed = Yaz0.Compress(result);
        using FileStream fs = File.Create(output);
        fs.Write(compressed.AsSpan());
    }

    private async Task GenerateAsync(string src)
    {
        Task[] tasks = [
            Task.Run(() => Parallel.ForEachAsync(Directory.EnumerateDirectories(src), async (folder, token) => {
                await GenerateAsync(folder);
            })),
            Task.Run(() => Parallel.ForEachAsync(Directory.EnumerateFiles(src), (file, token) => {
                // TODO: InjectFile(file);
                return ValueTask.CompletedTask;
            }))
        ];

        await Task.WhenAll(tasks);
    }
}
