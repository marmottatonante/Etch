using System.Buffers;
using System.Text;

namespace Etch;

public sealed class Canvas
{
    private readonly ArrayBufferWriter<byte> _buffer = new(8192);

    public void Move(Int2 position) => _buffer.Write(ANSI.MoveTo[position.Y][position.X]);
    public void Write(ReadOnlySpan<char> text) 
    {
        int max = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = _buffer.GetSpan(max);
        _buffer.Advance(Encoding.UTF8.GetBytes(text, span));
    }

    public ReadOnlySpan<byte> Span => _buffer.WrittenSpan;
    public void Clear() => _buffer.Clear();
    public void Clear(Rect rect)
    {
        var empty = new string(' ', rect.Size.X);
        for (int y = 0; y < rect.Size.Y; y++)
        {
            Move(rect.Position + new Int2(0, y));
            Write(empty.AsSpan());
        }
    }
}