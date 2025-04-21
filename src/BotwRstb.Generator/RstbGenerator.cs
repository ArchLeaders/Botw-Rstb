// ReSharper disable file StringLiteralTypo

using BotwRstb.Generator.Extensions;
using BotwRstb.Generator.ResourceTypes;
using BotwRstb.Generator.ResourceTypes.MemorySizes;
using CommunityToolkit.HighPerformance.Buffers;
using CsYaz0;
using CsYaz0.Marshalling;
using Kokuban;
using Revrs.Buffers;
using Revrs.Extensions;
using RstbLibrary;
using RstbLibrary.Helpers;

namespace BotwRstb.Generator;

public class RstbGenerator
{
    private readonly RstbGeneratorOptions _options;
    private readonly Rstb _vanilla;
    private readonly Rstb _result;

    public RstbGenerator(RstbGeneratorOptions options)
    {
        _options = options;

        using FileStream fs = File.OpenRead(options.SourceRstbFile);
        int size = Convert.ToInt32(fs.Length);

        using SpanOwner<byte> buffer = SpanOwner<byte>.Allocate(size);
        fs.ReadExactly(buffer.Span);

        int decompressedSize = Yaz0.GetDecompressedSize(buffer.Span);
        using SpanOwner<byte> decompressed = SpanOwner<byte>.Allocate(decompressedSize);
        Yaz0.Decompress(buffer.Span, decompressed.Span);

        _vanilla = Rstb.FromBinary(decompressed.Span);
        _result = Rstb.FromBinary(decompressed.Span);
    }

    public async Task GenerateAsync()
    {
        await Task.WhenAll(
            _options.InputContentFolders
                .Select(x => GenerateAsync(x.Path, x.Path, x.IsAoc))
        );

        if (_options.OutputRstbFile is not string output) {
            output = _options.SourceRstbFile;

            if (Path.GetDirectoryName(output) is string outputFolder) {
                Directory.CreateDirectory(outputFolder);
            }
        }

        byte[] result = _result.ToBinary();
        using DataMarshal compressed = Yaz0.Compress(result);
        await using FileStream fs = File.Create(output);
        fs.Write(compressed.AsSpan());
    }

    private async Task GenerateAsync(string src, string root, bool isAoc)
    {
        Task[] tasks = [
            Task.Run(() => Parallel.ForEachAsync(Directory.EnumerateDirectories(src), async (folder, token) => { await GenerateAsync(folder, root, isAoc); })),
            Task.Run(() => Parallel.ForEachAsync(Directory.EnumerateFiles(src), (file, token) => {
                uint resourceSize = GetResourceSize(file, root, isAoc, out string canonical);
                Insert(canonical, resourceSize);
                return ValueTask.CompletedTask;
            }))
        ];

        await Task.WhenAll(tasks);
    }

    public void Generate()
    {
        foreach ((string path, bool isAoc) in _options.InputContentFolders) {
            Generate(path, path, isAoc);
        }

        if (_options.OutputRstbFile is not string output) {
            output = _options.SourceRstbFile;

            if (Path.GetDirectoryName(output) is string outputFolder) {
                Directory.CreateDirectory(outputFolder);
            }
        }

        byte[] result = _result.ToBinary();
        using DataMarshal compressed = Yaz0.Compress(result);
        using FileStream fs = File.Create(output);
        fs.Write(compressed.AsSpan());
    }

    private void Generate(string src, string root, bool isAoc)
    {
        foreach (string folder in Directory.EnumerateDirectories(src)) {
            Generate(folder, root, isAoc);
        }

        foreach (string file in Directory.EnumerateFiles(src)) {
            uint resourceSize = GetResourceSize(file, root, isAoc, out string canonical);
            Insert(canonical, resourceSize);
        }
    }

    private uint GetResourceSize(string file, string root, bool isAoc, out string canonical)
    {
        Platform platform = _options.Platform;
        
        using FileStream fs = File.OpenRead(file);
        uint fileSize = GetFileSize(fs);
        
        canonical = file.ToCanonical(root, isAoc, out ReadOnlySpan<char> extension);
        BotwFactoryInfo info = BotwFactoryInfo.Get(canonical, extension, _options.Platform);

        Console.WriteLine(
            Chalk.Bold + canonical +
            Chalk.Faint + $" ({extension}) \n  " +
            Chalk.Underline + $"0x{fileSize:X}" + " | " +
            Chalk.BrightBlue + $"0x{info.Size:X} % 0x{info.Alignment:X} " +
            Chalk.Underline + $"[{info.ParseMode}]" +
            Chalk.BrightRed + (_options.Padding > 0 ? $" + {_options.Padding}" : "")
        );

        if (info.ParseMode is ParseMode.Simple) {
            return platform switch {
                Platform.WiiU => extension switch {
                    "esetlist" => fileSize.Round32() + AlignTo(0xE4 + info.Size + info.SimpleParseSize, info.Alignment),
                    "beventpack" => AlignTo(0xE4 + info.Size + info.SimpleParseSize, info.Alignment) + 0xE4,
                    _ => AlignTo(0xE4 + info.Size + info.SimpleParseSize, info.Alignment)
                },
                _ => AlignTo(0x168 + info.Size + info.SimpleParseSize, info.Alignment)
            };
        }
        
        uint rounded = platform switch {
            Platform.WiiU => fileSize.Round64(),
            _ => fileSize.Round32()
        };
        
        using ArraySegmentOwner<byte> dataOwner = ArraySegmentOwner<byte>.Allocate((int)fileSize);
        
        Span<byte> data = dataOwner.Segment;
        fs.ReadExactly(data);

        return extension switch {
            "baiprog" => rounded + AlignTo(
                AiProg.GetSize(data, platform),
                info.Alignment
            ),
            "baniminfo" => rounded + AlignTo(
                AnimInfo.GetSize(data, platform),
                info.Alignment
            ),
            "baslist" => rounded + AlignTo(
                AnimSequenceList.GetSize(data, platform),
                info.Alignment
            ),
            "bchemical" => rounded + AlignTo(
                Chemical.GetSize(data, platform),
                info.Alignment
            ),
            "bdrop" => rounded + AlignTo(
                Drop.GetSize(data, platform),
                info.Alignment
            ),
            "bfres" => BfresResource.EstimateSize(fileSize, platform),
            "bgparamlist" => rounded + AlignTo(
                GeneralParamList.GetSize(data, platform),
                info.Alignment
            ),
            "blifecondition" => rounded + AlignTo(
                LifeCondition.GetSize(data, platform),
                info.Alignment
            ),
            "bmodellist" => rounded + AlignTo(
                ModelList.GetSize(data, platform),
                info.Alignment
            ),
            "bphysics" => rounded + AlignTo(
                Physics.GetSize(data, platform),
                info.Alignment
            ),
            "bphyssb" => rounded + AlignTo(
                PhysicsSupportBone.GetSize(data, platform),
                info.Alignment
            ),
            "brecipe" => rounded + AlignTo(
                Recipe.GetSize(data, platform),
                info.Alignment
            ),
            "brgconfiglist" => rounded + AlignTo(
                RagdollConfigList.GetSize(data, platform),
                info.Alignment
            ),
            "bshop" => rounded + AlignTo(
                Shop.GetSize(data, platform),
                info.Alignment
            ),
            "bxml" => rounded + AlignTo(
                Xml.GetSize(data, platform),
                info.Alignment
            ),
            "hknm2" => rounded + platform switch {
                Platform.WiiU => 0x19CU,
                _ => 0x290,
            },
            "hksc" => rounded + platform switch {
                Platform.WiiU => 0x74CCU,
                _ => 0x9C00,
            },
            _ => AampResource.EstimateSize(fileSize, extension, platform)
        };
    }

    private void Insert(string canonical, uint size)
    {
        if (_result.OverflowTable.ContainsKey(canonical)) {
            lock (_result) {
                _result.OverflowTable[canonical] = size;
            }

            return;
        }

        uint hash = Crc32.Compute(canonical);
        lock (_result) {
            if (_result.HashTable.TryAdd(hash, size)) {
                return;
            }
        }
        
        // If the hash is not in the vanilla
        // RSTB it is a hash collision
        if (!_vanilla.HashTable.ContainsKey(hash)) {
            lock (_result) {
                _result.OverflowTable[canonical] = size;
            }

            return;
        }

        lock (_result) {
            _result.HashTable[hash] = size;
        }
    }

    private static uint GetFileSize(Stream fs)
    {
        try {
            uint magic = fs.Read<uint>();
            if (magic is Yaz0.MAGIC) return fs.Read<uint>();
        }
        finally {
            fs.Seek(0, SeekOrigin.Begin);
        }
        
        return (uint)fs.Length;
    }
    
    
    public static uint AlignTo(uint size, uint alignment)
    {
        return (uint)((size + (alignment - 1)) & -alignment);
    }
}