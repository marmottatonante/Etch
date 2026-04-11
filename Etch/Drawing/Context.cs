using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public readonly ref struct Context(ArrayBufferWriter<byte> buffer)
{
    public void Move(Int2 position)
    {
        var span = buffer.GetSpan(14);
        int written = 0;
        "\x1b["u8.CopyTo(span[written..]); written += 2;
        (position.Y + 1).TryFormat(span[written..], out int rw, default); written += rw;
        span[written++] = (byte)';';
        (position.X + 1).TryFormat(span[written..], out int cw, default); written += cw;
        span[written++] = (byte)'H';
        buffer.Advance(written);
    }

    public void Foreground(Color color)
    {
        byte bColor = (byte)color;
        var span = buffer.GetSpan(11);
        int written = 0;
        "\x1b[38;5;"u8.CopyTo(span[written..]); written += 7;
        bColor.TryFormat(span[written..], out int fw, default); written += fw;
        span[written++] = (byte)'m';
        buffer.Advance(written);
    }

    public void Background(Color color)
    {
        byte bColor = (byte)color;
        var span = buffer.GetSpan(11);
        int written = 0;
        "\x1b[48;5;"u8.CopyTo(span[written..]); written += 7;
        bColor.TryFormat(span[written..], out int fw, default); written += fw;
        span[written++] = (byte)'m';
        buffer.Advance(written);
    }

    public void Plot(byte glyph)
    {
        buffer.GetSpan(1)[0] = glyph;
        buffer.Advance(1);
    }

    public void Blit(ReadOnlySpan<byte> text)
    {
        var span = buffer.GetSpan(text.Length);
        text.CopyTo(span);
        buffer.Advance(text.Length);
    }

    public void Blit(ReadOnlySpan<char> text)
    {
        var span = buffer.GetSpan(text.Length);
        for (int i = 0; i < text.Length; i++)
            span[i] = (byte)text[i];
        buffer.Advance(text.Length);
    }

    public void Blank(int count)
    {
        var span = buffer.GetSpan(count);
        span[..count].Fill((byte)' ');
        buffer.Advance(count);
    }
}
