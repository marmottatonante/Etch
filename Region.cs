using System.Buffers;

namespace Etch;

public readonly ref struct Region(ArrayBufferWriter<byte> buffer, Rect bounds)
{
    private readonly ArrayBufferWriter<byte> _buffer = buffer;
    private readonly Rect _bounds = bounds;

    public readonly Rect Bounds => _bounds;

    public Region Reset() { ANSI.Reset(_buffer); return this; }
    public Region Foreground(Color color) { ANSI.Foreground(_buffer, color); return this; }
    public Region Background(Color color) { ANSI.Background(_buffer, color); return this; }

    public Region Write(ReadOnlySpan<char> text) => Write(Int2.Zero, text);
    public Region Write(Int2 position, ReadOnlySpan<char> text)
    {
        if(position.Y >= _bounds.Size.Y || position.Y < 0) return this;
        int max = _bounds.Size.X - position.X;
        if(max <= 0) return this;
        if(text.Length > max) text = text[..max];

        ANSI.Move(_buffer, _bounds.Position + position);
        ANSI.Write(_buffer, text);

        return this;
    }

    public Region Clear()
    {
        Span<char> spaces = stackalloc char[_bounds.Size.X];
        spaces.Fill(' ');
        for(int y = 0; y < _bounds.Size.Y; y++)
            Write((0, y), spaces);
        return this;
    }

    public Region Slice(Rect newRect)
    {
        var absolute = new Rect(_bounds.Position + newRect.Position, newRect.Size);
        var clipped = _bounds.Intersect(absolute) ?? Rect.Empty;
        return new Region(_buffer, clipped);
    }
}