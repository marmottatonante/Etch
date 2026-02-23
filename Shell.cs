using System.Buffers;
using System.Diagnostics;

namespace Etch;

public static class Shell
{
    private static readonly ArrayBufferWriter<byte> _buffer = new();
    private static readonly Stream _output = Console.OpenStandardOutput();
    private static readonly Stopwatch _stopwatch = new();

    public static bool AlternateBuffer
    {
        set => Console.Write(value ? "\x1b[?1049h" : "\x1b[?1049l");
    }

    public static double DeltaTime { get; private set; } = 0;
    public static double FrameTime { get; private set; } = 0;

    static Shell() => Platform.EnableAnsi();

    public static void RenderOnce(IControl root)
    {
        FrameTime = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        _buffer.Clear();
        ANSI.Clear(_buffer);

        Rect size = new(Int2.Zero, (Console.WindowWidth, Console.WindowHeight));
        root.Render(new Region(_buffer, size));

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        DeltaTime = _stopwatch.Elapsed.TotalSeconds;
    }

    public static void RenderFixed(IControl root, int framerate)
    {
        double targetSeconds = 1.0 / framerate;

        bool isRunning = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; isRunning = false; };

        while(isRunning)
        {
            Shell.RenderOnce(root);
            double remaining = targetSeconds - DeltaTime;
            if(remaining > 0) Thread.Sleep(TimeSpan.FromSeconds(remaining));
        }
    }

    public static void RenderFree(IControl root)
    {
        bool isRunning = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; isRunning = false; };
        while(isRunning) Shell.RenderOnce(root);
    }
}