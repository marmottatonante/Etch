using System.Buffers;
using System.Text;

namespace Etch;

public sealed class Canvas
{
    private readonly ArrayBufferWriter<byte> _buffer = new(8192);

    public void Move(int x, int y) => _buffer.Write(ANSI.MoveTo[y][x]);
    public void Write(ReadOnlySpan<char> text) 
    {
        int max = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = _buffer.GetSpan(max);
        _buffer.Advance(Encoding.UTF8.GetBytes(text, span));
    }

    public ReadOnlySpan<byte> Span => _buffer.WrittenSpan;
    public void Clear() => _buffer.Clear();
}