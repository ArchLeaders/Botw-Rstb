using System.Buffers;
using System.Runtime.CompilerServices;
using CsYaz0;
using Revrs;
using Revrs.Extensions;

namespace BotwRstb.Generator.Common;

public struct DataContainer : IDisposable, IAsyncDisposable
{
    public readonly ArraySegment<byte> Data = ArraySegment<byte>.Empty;
    
    private int _length;
    private readonly FileStream? _fs;
    private byte[]? _rented;
    
    public DataContainer(ArraySegment<byte> data)
    {
        _length = data.Count;
        Data = data;
    }
    
    public DataContainer(FileStream fs)
    {
        _length = (int)fs.Length;
        _fs = fs;
    }
    
    public static implicit operator DataContainer(FileStream fs) => new(fs);
    public static implicit operator DataContainer(ArraySegment<byte> data) => new(data);
    public static implicit operator ArraySegment<byte>(DataContainer container) => container.GetData();
    public static implicit operator Span<byte>(DataContainer container) => container.GetData();

    public ArraySegment<byte> GetData(bool decompress = false)
    {
        if (_rented is not null) {
            return new ArraySegment<byte>(_rented, 0, _length);
        }

        if (_fs is not null) {
            _rented = ArrayPool<byte>.Shared.Rent(_length);
            _fs.ReadExactly(_rented, 0, _length);

            if (decompress && _rented.AsSpan(0, _length) is var raw && raw.Read<uint>() is Yaz0.MAGIC) {
                _length = raw[0x4..0x8].Read<int>(Endianness.Big);
                byte[] decompressed = ArrayPool<byte>.Shared.Rent(_length);
                Yaz0.Decompress(raw, decompressed.AsSpan(0, _length));
                ArrayPool<byte>.Shared.Return(_rented);
                _rented = decompressed;
            }
            
            return new ArraySegment<byte>(_rented, 0, _length);
        }

        if (decompress && Data.AsSpan() is var span && span.Read<uint>() is Yaz0.MAGIC) {
            _length = span[0x4..0x8].Read<int>(Endianness.Big);
            _rented = ArrayPool<byte>.Shared.Rent(_length);
            Yaz0.Decompress(span, _rented.AsSpan(0, _length));
            return new ArraySegment<byte>(_rented, 0, _length);
        }

        return Data;
    }

    public FileStream? Stream {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get => _fs;
    }

    public void Dispose()
    {
        _fs?.Dispose();

        if (_rented is not null) {
            ArrayPool<byte>.Shared.Return(_rented);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_fs is not null) {
            await _fs.DisposeAsync();
        }
        
        if (_rented is not null) {
            ArrayPool<byte>.Shared.Return(_rented);
        }
    }
}