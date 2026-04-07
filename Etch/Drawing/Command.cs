using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public readonly struct Command(Command.Type type, Arena<byte>.Handle handle)
{
    public enum Type : byte { Move, Plot, Blit, Blank, Foreground, Background }

    public void Execute(ArrayBufferWriter<byte> output, Arena<byte> artifacts)
    {
        switch (type)
        {
            case Type.Move: ANSI.Move(output, artifacts.Read<Int2>(handle)[0]); break;
            case Type.Plot: ANSI.Plot(output, artifacts.Read(handle)[0]); break;
            case Type.Blit: ANSI.Blit(output, artifacts.Read(handle)); break;
            case Type.Blank: ANSI.Blank(output, artifacts.Read<int>(handle)[0]); break;
            case Type.Foreground: ANSI.Foreground(output, artifacts.Read<Color>(handle)[0]); break;
            case Type.Background: ANSI.Background(output, artifacts.Read<Color>(handle)[0]); break;
        }
    }
}