using Keystone;
using System.Text;

namespace Etch.Drawing;

public readonly ref struct Context(Arena<byte> artifacts, List<Command> commands)
{
    public void Move(Int2 position)
    {
        var handle = artifacts.Allocate<Int2>(1);
        artifacts.Write(handle, position);
        commands.Add(new Command(Command.Type.Move, handle));
    }

    public void Plot(byte glyph)
    {
        var handle = artifacts.Allocate(1);
        artifacts.Write(handle, glyph);
        commands.Add(new Command(Command.Type.Plot, handle));
    }

    public void Blit(ReadOnlySpan<char> text)
    {
        int maxBytes = Encoding.UTF8.GetMaxByteCount(text.Length);
        var handle = artifacts.Allocate(maxBytes);
        var span = artifacts.GetSpan(handle);
        int written = Encoding.UTF8.GetBytes(text, span);
        commands.Add(new Command(Command.Type.Blit, handle.Trim(written)));
    }

    public void Blank(int count)
    {
        var handle = artifacts.Allocate<int>(1);
        artifacts.Write(handle, count);
        commands.Add(new Command(Command.Type.Blank, handle));
    }

    public void Foreground(Color color)
    {
        var handle = artifacts.Allocate<Color>(1);
        artifacts.Write(handle, color);
        commands.Add(new Command(Command.Type.Foreground, handle));
    }

    public void Background(Color color)
    {
        var handle = artifacts.Allocate<Color>(1);
        artifacts.Write(handle, color);
        commands.Add(new Command(Command.Type.Background, handle));
    }
}