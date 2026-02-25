using System.Buffers;

namespace Etch;

public static partial class Shell
{
    private static readonly ArrayBufferWriter<byte> _buffer = new();
    private static readonly Stream _output = Console.OpenStandardOutput();

    public static Rect Size => new(Int2.Zero, (Console.WindowWidth, Console.WindowHeight));
    private static Rect _lastSize = Rect.Empty;
    public static Scene Scene { get; set; } = new((new Label("Welcome to Etch!"), Layouts.Center));
    private static Scene _lastScene = null;

    public static bool AlternateBuffer { set => Console.Write(value ? "\x1b[?1049h" : "\x1b[?1049l"); }
    public static bool Cursor { set => Console.Write(value ? "\x1b[?25h" : "\x1b[?25l"); }

    static Shell() => Platform.EnableAnsi();

    public static void Render()
    {
        Metrics.StartDraw();

        _buffer.Clear();

        bool hasResized = _lastSize != Size;
        bool hasChanged = _lastScene != Scene;
        if (hasResized || hasChanged)
        {
            _lastSize = Size;
            _lastScene = Scene;

            ANSI.Clear(_buffer);
            Scene.Arrange(_lastSize);
            Scene.Draw(_buffer);
        }
        else Scene.Update(_buffer);

        Metrics.StartFlush();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        Metrics.EndFrame();
    }

    public static void Run(int framerate = 0)
    {
        double target = framerate > 0 ? 1.0 / framerate : 0;
        bool running = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; running = false; };
        while(running)
        {
            Render();
            double remaining = target - Metrics.DeltaTime;
            if(remaining > 0) Thread.Sleep(TimeSpan.FromSeconds(remaining));
        }
    }
}