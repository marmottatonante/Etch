using System.Diagnostics;

namespace Etch;

public class Profiler
{
    public bool Active { get; set; } = false;

    private readonly Stopwatch _stopwatch = new();

    public double DrawTime { get; private set; }
    public double FlushTime { get; private set; }
    public double DeltaTime => DrawTime + FlushTime;

    internal void StartDraw()
    {
        if (!Active) return;
        _stopwatch.Restart();
    }

    internal void StartFlush()
    {
        if (!Active) return;
        DrawTime = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();
    }

    internal void Stop()
    {
        if (!Active) return;
        FlushTime = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Stop();
    }
}