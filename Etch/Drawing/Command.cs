using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public readonly struct Command(Command.Type type, Arena<byte>.Handle handle)
{
    public enum Type : byte
    {
        Move,
        Plot,
        Blit,
        Blank,
        Foreground,
        Background,
    }

    public readonly Type Kind = type;
    public readonly Arena<byte>.Handle Handle = handle;

    public void Execute(ArrayBufferWriter<byte> output, Arena<byte> artifacts)
    {
        switch (Kind)
        {
            case Type.Move:
                ANSI.Move(output, artifacts.Read<Int2>(Handle)[0]);
                break;
            case Type.Plot:
                ANSI.Plot(output, artifacts.Read(Handle)[0]);
                break;
            case Type.Blit:
                ANSI.Blit(output, artifacts.Read(Handle));
                break;
            case Type.Blank:
                ANSI.Blank(output, artifacts.Read<int>(Handle)[0]);
                break;
            case Type.Foreground:
                ANSI.Foreground(output, artifacts.Read<Color>(Handle)[0]);
                break;
            case Type.Background:
                ANSI.Background(output, artifacts.Read<Color>(Handle)[0]);
                break;
        }
    }
}