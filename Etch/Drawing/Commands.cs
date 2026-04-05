using Keystone;
using System.Buffers;
using System.Text;

namespace Etch.Drawing;

public readonly record struct Plot(Int2 Position, byte Glyph, Color Foreground = Color.White, Color Background = Color.Black) : ICommand
{
    public void Draw(ArrayBufferWriter<byte> buffer)
    {
        ANSI.Move(buffer, Position);
        ANSI.Foreground(buffer, Foreground);
        ANSI.Background(buffer, Background);
        buffer.GetSpan(1)[0] = Glyph;
        buffer.Advance(1);
    }

    public void Undraw(ArrayBufferWriter<byte> buffer)
    {
        ANSI.Move(buffer, Position);
        buffer.GetSpan(1)[0] = (byte)' ';
        buffer.Advance(1);
    }
}

public readonly record struct Blit(Int2 Position, string Text, Color Foreground = Color.White, Color Background = Color.Black) : ICommand
{
    public void Draw(ArrayBufferWriter<byte> buffer)
    {
        ANSI.Move(buffer, Position);
        ANSI.Foreground(buffer, Foreground);
        ANSI.Background(buffer, Background);
        int bytes = Encoding.UTF8.GetMaxByteCount(Text.Length);
        var span = buffer.GetSpan(bytes);
        buffer.Advance(Encoding.UTF8.GetBytes(Text.AsSpan(), span));
    }

    public void Undraw(ArrayBufferWriter<byte> buffer)
    {
        ANSI.Move(buffer, Position);
        int count = Text.Length;
        var span = buffer.GetSpan(count);
        span[..count].Fill((byte)' ');
        buffer.Advance(count);
    }
}