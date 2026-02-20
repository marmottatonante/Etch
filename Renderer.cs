using System.Diagnostics;

namespace Etch;

public record struct Context(Canvas Canvas, Rect Bounds);
public sealed class Renderer : IDisposable
{
    private readonly Canvas _canvas = new();
    private readonly Stream _output = Console.OpenStandardOutput();

    public double DeltaTime { get; private set; } = 0;
    public double FPS => 1.0 / DeltaTime;

    public static class Errors
    {
        public static readonly IControl NoContent = new Center(new Label("No Content has been defined."));
        public static readonly IControl InsufficientSize = new Center(new Label("!"));
    }
    
    public IControl? Content { get; set; } = null;
    
    public Renderer() => Console.Write("\x1b[?25l\x1b[2J\x1b[?7l");
    public void Dispose() => Console.Write("\x1b[?25h\x1b[2J\x1b[H\x1b[?7h");

    public void RenderOnce()
    {
        _canvas.Clear();

        Int2 windowSize = new(Console.WindowWidth, Console.WindowHeight);
        IControl target = Content ?? Errors.NoContent;

        Int2 targetSize = target.Measure(windowSize);
        if(targetSize > windowSize) target = Errors.InsufficientSize;
        target.Render(new(_canvas, new((0, 0), targetSize)));

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