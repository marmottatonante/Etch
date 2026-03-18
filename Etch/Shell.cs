using System.Buffers;
using Pith.Geometry;

namespace Etch;

public static partial class Shell
{
    private static readonly ArrayBufferWriter<byte> _buffer = new();
    private static readonly Stream _output = Console.OpenStandardOutput();

    public static Int2 ScreenSize => new(Console.WindowWidth, Console.WindowHeight);

    public static bool AlternateBuffer { set => Console.Write(value ? "\x1b[?1049h" : "\x1b[?1049l"); }
    public static bool CursorVisible { set => Console.Write(value ? "\x1b[?25h" : "\x1b[?25l"); }

    public static Renderable? Root { get; set; }

    public static void Clear() => Console.Write("\x1b[2J");
    public static void Render()
    {
        if (Root is null) return;

        Metrics.StartDraw();

        _buffer.Clear();

        Int2 size = Root.Measure(ScreenSize);
        Rect rect = new(Int2.Zero, size);
        Surface surface = new(_buffer, rect);

        Root.Arrange(surface);
        Root.Render();

        Metrics.StartFlush();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        Metrics.Stop();
    }
}