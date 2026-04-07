using Keystone;
using System.Text;

namespace Etch.Drawing;

public readonly ref struct Context(Arena<byte> artifacts, List<Command> commands)
{
    public void Move(Int2 position) =>
        commands.Add(new Command(Command.Type.Move, artifacts.Write(position)));

    public void Plot(byte glyph) => 
        commands.Add(new Command(Command.Type.Plot, artifacts.Write(glyph)));

    public void Blit(ReadOnlySpan<char> text)
    {
        int byteCount = Encoding.UTF8.GetByteCount(text);
        Span<byte> utf8Buffer = stackalloc byte[512];
        int written = Encoding.UTF8.GetBytes(text, utf8Buffer);
        var handle = artifacts.Write(utf8Buffer[..written]);
        commands.Add(new Command(Command.Type.Blit, handle));
    }

    public void Blank(int count) =>
        commands.Add(new Command(Command.Type.Blank, artifacts.Write(count)));

    public void Foreground(Color color) => 
        commands.Add(new Command(Command.Type.Foreground, artifacts.Write(color)));

    public void Background(Color color) =>
        commands.Add(new Command(Command.Type.Background, artifacts.Write(color)));
}