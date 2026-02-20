using System.Buffers;
using System.Diagnostics;

namespace Etch;

public sealed class Renderer : IDisposable
{
    private readonly ArrayBufferWriter<byte> _buffer = new();
    private readonly Stream _output = Console.OpenStandardOutput();

    private readonly record struct Slot(IControl Control, Layout Layout, Rect Current, Rect Previous);
    private readonly List<Slot> _controls = [];
    public void Add(IControl control, Layout layout) =>
        _controls.Add(new(control, layout, Rect.Empty, Rect.Empty)); 

    public double DeltaTime { get; private set; } = 0;
    public double FPS { get; private set; }
    private const double Smoothing = 0.9;

    public Renderer() => Console.Write("\x1b[?25l\x1b[2J");
    public void Dispose() => Console.Write("\x1b[?25h\x1b[2J\x1b[H");

    public void RenderOnce()
    {
        _buffer.Clear();

        Rect screenRect = new(Int2.Zero, (Console.WindowWidth, Console.WindowHeight));
        for(int i = 0; i < _controls.Count; i++)
        {
            IControl control = _controls[i].Control;
            Layout layout = _controls[i].Layout;
            Rect newPrevious = _controls[i].Current;
            Rect newCurrent = layout(screenRect, control.Size);

            if(newPrevious != Rect.Empty) new Region(_buffer, newPrevious).Clear();

            control.Render(new(_buffer, newCurrent));
            File.AppendAllText("debug.log", $"{control.GetType().Name}: bounds={newCurrent}\n");
            _controls[i] = new Slot(control, layout, newCurrent, newPrevious);
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
            double currentFPS = 1.0 / DeltaTime;
            FPS = FPS * Smoothing + currentFPS * (1.0 - Smoothing);
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
            double currentFPS = 1.0 / DeltaTime;
            FPS = FPS * Smoothing + currentFPS * (1.0 - Smoothing);
        }
    }
}