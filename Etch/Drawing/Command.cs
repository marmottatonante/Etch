using Keystone;
using System.Buffers;
using System.Text;

namespace Etch.Drawing;

public readonly record struct Command(Command.Kind Type, Int2 Position, byte PlotData, string? BlitData, int BlankData, Color Foreground, Color Background)
{
    public enum Kind { Plot, Blit, Blank }

    public void Execute(ArrayBufferWriter<byte> buffer)
    {
        switch (Type)
        {
            case Kind.Plot:
            {
                ANSI.Move(buffer, Position);
                ANSI.Foreground(buffer, Foreground);
                ANSI.Background(buffer, Background);
                buffer.GetSpan(1)[0] = PlotData;
                buffer.Advance(1);
                break;
            }
            case Kind.Blit:
            {
                ANSI.Move(buffer, Position);
                ANSI.Foreground(buffer, Foreground);
                ANSI.Background(buffer, Background);
                int bytes = Encoding.UTF8.GetMaxByteCount(BlitData!.Length);
                var span = buffer.GetSpan(bytes);
                buffer.Advance(Encoding.UTF8.GetBytes(BlitData.AsSpan(), span));
                break;
            }
            case Kind.Blank:
            {
                ANSI.Move(buffer, Position);
                var span = buffer.GetSpan(BlankData);
                span[..BlankData].Fill((byte)' ');
                buffer.Advance(BlankData);
                break;
            }
        }
    }

    public static Command Plot(Int2 position, byte glyph, Color fg = Color.White, Color bg = Color.Black) => new(Kind.Plot, position, glyph, null, 0, fg, bg);
    public static Command Blit(Int2 position, string text, Color fg = Color.White, Color bg = Color.Black) => new(Kind.Blit, position, default, text, 0, fg, bg);
    public static Command Blank(Int2 position, int width) => new(Kind.Blank, position, default, null, width, default, default);
}