using System.Buffers;
using System.Text;

namespace Etch;

public static class ANSI
{
    public static void Reset(ArrayBufferWriter<byte> buffer) => buffer.Write("\x1b[0m"u8);
    private static void Color(ArrayBufferWriter<byte> buffer, ReadOnlySpan<byte> prefix, Color color)
    {
        byte bColor = (byte)color;
        var span = buffer.GetSpan(11);
        int written = 0;
        prefix.CopyTo(span[written..]); written += prefix.Length;
        bColor.TryFormat(span[written..], out int fw, default, null); written += fw;
        span[written++] = (byte)'m';
        buffer.Advance(written);
    }
    public static void Foreground(ArrayBufferWriter<byte> buffer, Color color) => ANSI.Color(buffer, "\x1b[38;5;"u8, color);
    public static void Background(ArrayBufferWriter<byte> buffer, Color color) => ANSI.Color(buffer, "\x1b[48;5;"u8, color);
    public static void Move(ArrayBufferWriter<byte> buffer, Int2 position)
    {
        // \x1b[{row};{col}H
        // Max length: "\x1b[" (2) + 5 digit row + ";" + 5 digit col + "H" = 14 bytes
        var span = buffer.GetSpan(14);
        int written = 0;
        "\x1b["u8.CopyTo(span[written..]); written += 2;
        (position.Y + 1).TryFormat(span[written..], out int rw, default, null); written += rw;
        span[written++] = (byte)';';
        (position.X + 1).TryFormat(span[written..], out int cw, default, null); written += cw;
        span[written++] = (byte)'H';
        buffer.Advance(written);
    }
    public static void Write(ArrayBufferWriter<byte> buffer, byte b)
    {
        buffer.GetSpan(1)[0] = b;
        buffer.Advance(1);
    }
    public static void Write(ArrayBufferWriter<byte> buffer, ReadOnlySpan<char> text)
    {
        int bytes = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = buffer.GetSpan(bytes);
        buffer.Advance(Encoding.UTF8.GetBytes(text, span));
    }
}