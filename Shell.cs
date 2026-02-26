using System.Buffers;

namespace Etch;

public static partial class Shell
{
    private static readonly ArrayBufferWriter<byte> _buffer = new();
    private static readonly Stream _output = Console.OpenStandardOutput();

    static Shell() => Platform.EnableAnsi();

    public static Int2 Size => new(Console.WindowWidth, Console.WindowHeight);

    public static bool AlternateBuffer { set => Console.Write(value ? "\x1b[?1049h" : "\x1b[?1049l"); }
    public static bool CursorVisible { set => Console.Write(value ? "\x1b[?25h" : "\x1b[?25l"); }

    public static Control? Root { get; set; }

    public static void Batch() { Root?.Changed -= Render; }
    public static void Render()
    {
        if (Root is null) return;
        Root.Changed -= Render;
        Root.Changed += Render;

        _buffer.Clear();
        Int2 size = Root.Measure(Size);
        Region region = new(_buffer, new(Int2.Zero, size));
        Root.Render(region);

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();
    }
}