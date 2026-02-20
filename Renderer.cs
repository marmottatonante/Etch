using System.Buffers;
using System.Diagnostics;

namespace Etch;

public sealed class Renderer : IDisposable
{
    private readonly ArrayBufferWriter<byte> _buffer = new();
    private readonly Stream _output = Console.OpenStandardOutput();

    public readonly record struct Renderable(IControl Control, Rect Current, Rect Previous);
    private readonly List<Renderable> _renderables = [];
    public void Add(Renderable renderable) => _renderables.Add(renderable);

    public double DeltaTime { get; private set; } = 0;
    public double FPS => 1.0 / DeltaTime;

    public Renderer() => Console.Write("\x1b[?25l\x1b[2J");
    public void Dispose() => Console.Write("\x1b[?25h\x1b[2J\x1b[H");

    public void RenderOnce()
    {
        _buffer.Clear();

        foreach(var renderable in _renderables)
        {
            Region region = new(_buffer, renderable.Current.Position);
            renderable.Control.Render(region);
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
        }
    }
}