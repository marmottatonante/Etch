using System.Buffers;
using System.Text;

namespace Etch;

public readonly ref struct Region(ArrayBufferWriter<byte> buffer, Rect bounds)
{
    public const byte ClipIndicator = (byte)'>';
    private readonly ArrayBufferWriter<byte> _buffer = buffer;
    private readonly Rect _bounds = bounds;

    public readonly Rect Bounds => _bounds;

    public Region Reset()
    {
        _buffer.Write(ANSI.Reset);
        return this;
    }
    public Region Color(byte foreground, byte background)
    {
        _buffer.Write(ANSI.Color(foreground, background));
        return this;
    }
    public Region Write(ReadOnlySpan<char> text) => Write(Int2.Zero, text);
    public Region Write(Int2 position, ReadOnlySpan<char> text) 
    {
        if(position.Y >= _bounds.Size.Y || position.Y < 0) return this;
        int max = _bounds.Size.X - position.X;
        if(max <= 0) return this;
        bool clipped = text.Length > max;
        if(clipped) text = text[..(max - 1)];

        _buffer.Write(ANSI.MoveTo(_bounds.Position + position));
        int bytes = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = _buffer.GetSpan(bytes);
        _buffer.Advance(Encoding.UTF8.GetBytes(text, span));
        if(clipped) { _buffer.GetSpan(1)[0] = ClipIndicator; _buffer.Advance(1); }

        return this;
    }
    public Region Slice(Rect newRect)
    {
        var absolute = new Rect(_bounds.Position + newRect.Position, newRect.Size);
        var clipped = _bounds.Intersect(absolute) ?? Rect.Empty;
        return new Region(_buffer, clipped);
    }
}