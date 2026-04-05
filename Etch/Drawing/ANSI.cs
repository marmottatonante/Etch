using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public static class ANSI
{
    public static void Move(ArrayBufferWriter<byte> buffer, Int2 position)
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

    public static void Foreground(ArrayBufferWriter<byte> buffer, Color color)
    {
        byte bColor = (byte)color;
        var span = buffer.GetSpan(11);
        int written = 0;
        "\x1b[38;5;"u8.CopyTo(span[written..]); written += 7;
        bColor.TryFormat(span[written..], out int fw, default); written += fw;
        span[written++] = (byte)'m';
        buffer.Advance(written);
    }

    public static void Background(ArrayBufferWriter<byte> buffer, Color color)
    {
        byte bColor = (byte)color;
        var span = buffer.GetSpan(11);
        int written = 0;
        "\x1b[48;5;"u8.CopyTo(span[written..]); written += 7;
        bColor.TryFormat(span[written..], out int fw, default); written += fw;
        span[written++] = (byte)'m';
        buffer.Advance(written);
    }
}