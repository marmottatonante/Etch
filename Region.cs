using System.Buffers;
using System.Text;

namespace Etch;

public ref struct Region
{
    private readonly ArrayBufferWriter<byte> _buffer;
    private readonly Rect _bounds;
    private Int2 _cursor;

    public Region(ArrayBufferWriter<byte> buffer, Rect bounds)
    {
        _buffer = buffer;
        _bounds = bounds;
        Move(Int2.Zero);
    }

    public Region Move(Int2 position)
    {
        _cursor = new Int2(
            Math.Clamp(position.X, 0, _bounds.Size.X - 1),
            Math.Clamp(position.Y, 0, _bounds.Size.Y - 1)
        );
        _buffer.Write(ANSI.MoveTo(_bounds.Position + _cursor));
        return this;
    }
    public Region Write(ReadOnlySpan<char> text) 
    {
        int leftClip = Math.Max(0, -(_bounds.Position.X + _cursor.X));
        if (leftClip >= text.Length) return this;
        text = text[leftClip..];

        int remaining = _bounds.Size.X - _cursor.X;
        if (remaining <= 0) return this;
        if (text.Length > remaining) text = text[..remaining];

        int max = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = _buffer.GetSpan(max);
        _buffer.Advance(Encoding.UTF8.GetBytes(text, span));
        _cursor = new Int2(_cursor.X + text.Length, _cursor.Y);
        return this;
    }
    public Region Clear()
    {
        var spaces = new string(' ', _bounds.Size.X);
        for (int y = 0; y < _bounds.Size.Y; y++)
        {
            Move((0, y));
            Write(spaces);
        }
        return this;
    }
}