using System.Diagnostics;

namespace Etch;

public record struct Context(Canvas Canvas, Rect Bounds);
public sealed class Renderer : IDisposable
{
    private readonly Canvas _canvas = new();
    private readonly Stream _output = Console.OpenStandardOutput();

    public double DeltaTime { get; private set; } = 0;
    public double FPS => 1.0 / DeltaTime;

    public IControl? Content { get; set; } = null;
    public IControl? Overlay { get; set; } = null;

    public Renderer() => Console.Write("\x1b[?25l\x1b[2J\x1b[?7l");
    public void Dispose() => Console.Write("\x1b[?25h\x1b[2J\x1b[H\x1b[?7h");

    public void RenderOnce()
    {
        _canvas.Clear();

        Int2? rootSize = Content?.Measure((Console.WindowWidth, Console.WindowHeight));
        if (rootSize is not null) Content?.Render(new Context(_canvas, new(Int2.Zero, rootSize.Value)));

        Int2? overlaySize = Overlay?.Measure((Console.WindowWidth, Console.WindowHeight));
        if(overlaySize is not null) Overlay?.Render(new Context(_canvas, new(Int2.Zero, overlaySize.Value)));

        _output.Write(_canvas.Span);
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