using System.Buffers;
using Pith.Geometry;

namespace Etch;

public readonly struct Surface(ArrayBufferWriter<byte> buffer, Rect bounds)
{
    private readonly ArrayBufferWriter<byte> _buffer = buffer;
    private readonly Rect _bounds = bounds;

    public Rect Bounds => _bounds;

    public void Reset() => ANSI.Reset(_buffer);
    public void Foreground(Color color) => ANSI.Foreground(_buffer, color);
    public void Background(Color color) => ANSI.Background(_buffer, color);

    public void Write(ReadOnlySpan<char> text) => Write(Int2.Zero, text);
    public void Write(Int2 position, ReadOnlySpan<char> text)
    {
        if(position.Y >= _bounds.Size.Y || position.Y < 0) return;
        int max = _bounds.Size.X - position.X;
        if(max <= 0) return;
        if(text.Length > max) text = text[..max];

        ANSI.Move(_buffer, _bounds.Position + position);
        ANSI.Write(_buffer, text);
    }

    public void Clear()
    {
        Span<char> spaces = stackalloc char[_bounds.Size.X];
        spaces.Fill(' ');
        for(int y = 0; y < _bounds.Size.Y; y++)
            Write((0, y), spaces);
    }

    public Surface Slice(Rect newRect)
    {
        var absolute = new Rect(_bounds.Position + newRect.Position, newRect.Size);
        var clipped = _bounds.Intersect(absolute) ?? Rect.Empty;
        return new Surface(_buffer, clipped);
    }
}