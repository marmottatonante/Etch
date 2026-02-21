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

    public double DeltaTime { get; private set; } = 0;
    public double FPS { get; private set; }
    public double MaxFPS { get; private set; } = double.MinValue;
    public double MinFPS { get; private set; } = double.MaxValue;

    private static readonly Label Placeholder = new("!");

    public Renderer() => Console.Write("\x1b[?25l\x1b[2J");
    public void Dispose() => Console.Write("\x1b[?25h\x1b[2J\x1b[H");

    public void RenderOnce()
    {
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
    }

    public void RenderAt(int targetFPS)
    {
        Stopwatch stopwatch = new();
        double targetSeconds = 1.0 / targetFPS;
        
        bool isRunning = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; isRunning = false; };

        while (isRunning)
        {
            stopwatch.Restart();
            this.RenderOnce();

            double elapsed = stopwatch.Elapsed.TotalSeconds;
            double remaining = targetSeconds - elapsed;
            if(remaining > 0) Thread.Sleep(TimeSpan.FromSeconds(remaining));

            DeltaTime = stopwatch.Elapsed.TotalSeconds;
            FPS = 1.0 / DeltaTime;
            if(FPS > MaxFPS) MaxFPS = FPS;
            if(FPS < MinFPS) MinFPS = FPS;
        }
    }

    public void RenderFree()
    {
        Stopwatch stopwatch = new();
        
        bool isRunning = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; isRunning = false; };

        while (isRunning)
        {
            stopwatch.Restart();
            this.RenderOnce();

            DeltaTime = stopwatch.Elapsed.TotalSeconds;
            FPS = 1.0 / DeltaTime;
            if(FPS > MaxFPS) MaxFPS = FPS;
            if(FPS < MinFPS) MinFPS = FPS;
        }
    }
}