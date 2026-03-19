using Keystone.Observables;
using Keystone.Primitives;
using System.Buffers;
using System.Text;

namespace Etch;

public sealed class Canvas(Stream output, Int2 size)
{
    private readonly ArrayBufferWriter<byte> _buffer = new();
    private readonly Stream _output = output;

    public readonly Property<Int2> Size = new(size);

    public Canvas Clear() { _buffer.Write("\x1b[2J"u8); return this; }
    public Canvas Reset() { _buffer.Write("\x1b[0m"u8); return this; }

    private Canvas Color(ReadOnlySpan<byte> prefix, Color color)
    {
        byte bColor = (byte)color;
        var span = _buffer.GetSpan(11);
        int written = 0;
        prefix.CopyTo(span[written..]); written += prefix.Length;
        bColor.TryFormat(span[written..], out int fw, default, null); written += fw;
        span[written++] = (byte)'m';
        _buffer.Advance(written);
        return this;
    }

    public Canvas Foreground(Color color) => Color("\x1b[38;5;"u8, color);
    public Canvas Background(Color color) => Color("\x1b[48;5;"u8, color);

    public Canvas Move(Int2 position)
    {
        var span = _buffer.GetSpan(14);
        int written = 0;
        "\x1b["u8.CopyTo(span[written..]); written += 2;
        (position.Y + 1).TryFormat(span[written..], out int rw, default, null); written += rw;
        span[written++] = (byte)';';
        (position.X + 1).TryFormat(span[written..], out int cw, default, null); written += cw;
        span[written++] = (byte)'H';
        _buffer.Advance(written);
        return this;
    }

    public Canvas Write(byte b) { _buffer.GetSpan(1)[0] = b; _buffer.Advance(1); return this; }
    public Canvas Write(ReadOnlySpan<char> text)
    {
        int bytes = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = _buffer.GetSpan(bytes);
        _buffer.Advance(Encoding.UTF8.GetBytes(text, span));
        return this;
    }

    public void Flush()
    {
        _output.Write(_buffer.WrittenSpan);
        _output.Flush();
        _buffer.Clear();
    }
}