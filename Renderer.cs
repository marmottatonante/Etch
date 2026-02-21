using System.Buffers;
using System.Diagnostics;

namespace Etch;

public sealed class Renderer : IDisposable
{
    private readonly ArrayBufferWriter<byte> _buffer = new();
    private readonly Stream _output = Console.OpenStandardOutput();

    private readonly List<(IControl Control, Layout Layout)> _controls = [];
    public void Add(IControl control, Layout layout) =>
        _controls.Add(new(control, layout)); 

    private readonly Stopwatch _stopwatch = new();
    public double DeltaTime { get; private set; } = 0;
    public double FrameTime { get; private set; } = 0;

    private static readonly Label Placeholder = new("!");

    public Renderer() => Console.Write("\x1b[?25l\x1b[?1049h");
    public void Dispose() => Console.Write("\x1b[?25l\x1b[?1049l");

    public void RenderOnce()
    {
        FrameTime = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        _buffer.Clear();
        _buffer.Write("\x1b[2J"u8);

        Rect screenRect = new(Int2.Zero, (Console.WindowWidth, Console.WindowHeight));
        foreach(var (Control, Layout) in _controls)
        {
            Rect rect = Layout(screenRect, Control.Size);
            if(screenRect.Contains(rect)) Control.Render(new Region(_buffer, rect));
            else Placeholder.Render(new Region(_buffer, Layout(screenRect, Placeholder.Size)));
        }

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        DeltaTime = _stopwatch.Elapsed.TotalSeconds;
    }

    public void RenderAt(int targetFPS)
    {
        double targetSeconds = 1.0 / targetFPS;

        bool isRunning = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; isRunning = false; };

        while (isRunning)
        {
            this.RenderOnce();
            double remaining = targetSeconds - DeltaTime;
            if(remaining > 0) Thread.Sleep(TimeSpan.FromSeconds(remaining));
        }
    }

    public void RenderFree()
    {
        bool isRunning = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; isRunning = false; };
        while (isRunning) this.RenderOnce();
    }
}