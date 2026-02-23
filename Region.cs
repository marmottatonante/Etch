using System.Buffers;
using System.Text;

namespace Etch;

public readonly ref struct Region(ArrayBufferWriter<byte> main, ArrayBufferWriter<byte> error, Rect bounds)
{
    private readonly ArrayBufferWriter<byte> _main = main;
    private readonly ArrayBufferWriter<byte> _error = error;
    private readonly Rect _bounds = bounds;

    public readonly Rect Bounds => _bounds;

    public Region Reset() { ANSI.Reset(_main); return this; }
    public Region Foreground(Color color) { ANSI.Foreground(_main, color); return this; }
    public Region Background(Color color) { ANSI.Background(_main, color); return this; }

    public Region Write(ReadOnlySpan<char> text) => Write(Int2.Zero, text);
    public Region Write(Int2 position, ReadOnlySpan<char> text)
    {
        if(position.Y >= _bounds.Size.Y || position.Y < 0) return this;
        int max = _bounds.Size.X - position.X;
        if(max <= 0) return this;
        if(text.Length > max) text = text[..max];

        ANSI.Move(_main, position);
        ANSI.Write(_main, text);

        return this;
    }

    private Region Write(Error error)
    {
        ANSI.Background(_error, Color.Red);
        ANSI.Foreground(_error, Color.White);
        ANSI.Move(_error, (0, 0));
        ((byte)error).TryFormat(_error.GetSpan(3), out int written, default, null);
        _error.Advance(written);
        ANSI.Reset(_error);

        return this;
    }

    public Region Slice(Rect newRect)
    {
        var absolute = new Rect(_bounds.Position + newRect.Position, newRect.Size);
        var clipped = _bounds.Intersect(absolute) ?? Rect.Empty;

        if(clipped != absolute && !clipped.IsEmpty)
            Write(Error.Clipping);

        return new Region(_main, _error, clipped);
    }
}