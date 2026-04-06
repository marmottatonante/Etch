using Keystone;
using System.Buffers;
using System.Text;

namespace Etch.Drawing;

public readonly ref struct Context(ArrayBufferWriter<byte> buffer)
{
    private readonly ArrayBufferWriter<byte> _buffer = buffer;

    public void Move(Int2 position)
    {
        var span = _buffer.GetSpan(14);
        int written = 0;
        "\x1b["u8.CopyTo(span[written..]); written += 2;
        (position.Y + 1).TryFormat(span[written..], out int rw, default); written += rw;
        span[written++] = (byte)';';
        (position.X + 1).TryFormat(span[written..], out int cw, default); written += cw;
        span[written++] = (byte)'H';
        _buffer.Advance(written);
    }

    public void Foreground(Color color)
    {
        byte bColor = (byte)color;
        var span = _buffer.GetSpan(11);
        int written = 0;
        "\x1b[38;5;"u8.CopyTo(span[written..]); written += 7;
        bColor.TryFormat(span[written..], out int fw, default); written += fw;
        span[written++] = (byte)'m';
        _buffer.Advance(written);
    }

    public void Background(Color color)
    {
        byte bColor = (byte)color;
        var span = _buffer.GetSpan(11);
        int written = 0;
        "\x1b[48;5;"u8.CopyTo(span[written..]); written += 7;
        bColor.TryFormat(span[written..], out int fw, default); written += fw;
        span[written++] = (byte)'m';
        _buffer.Advance(written);
    }

    public void Plot(byte glyph)
    {
        _buffer.GetSpan(1)[0] = glyph;
        _buffer.Advance(1);
    }

    public void Blit(ReadOnlySpan<char> text)
    {
        int bytes = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = _buffer.GetSpan(bytes);
        _buffer.Advance(Encoding.UTF8.GetBytes(text, span));
    }

    public void Blank(int count)
    {
        var span = _buffer.GetSpan(count);
        span[..count].Fill((byte)' ');
        _buffer.Advance(count);
    }
}