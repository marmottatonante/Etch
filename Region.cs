using System.Buffers;
using System.Text;

namespace Etch;

public readonly ref struct Region
{
    private readonly ArrayBufferWriter<byte> _buffer;
    private readonly Rect _bounds;

    public Region(ArrayBufferWriter<byte> buffer, Rect bounds)
    {
        _buffer = buffer;
        _bounds = bounds;
        Move(Int2.Zero);
    }

    public Region Move(Int2 position)
    {
        _buffer.Write(ANSI.MoveTo(_bounds.Position + position));
        return this;
    }
    public Region Write(ReadOnlySpan<char> text) 
    {
        int max = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = _buffer.GetSpan(max);
        _buffer.Advance(Encoding.UTF8.GetBytes(text, span));
        return this;
    }
}