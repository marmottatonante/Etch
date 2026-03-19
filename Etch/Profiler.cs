using System.Diagnostics;

namespace Etch;

public class Profiler
{
    public bool Active { get; set; } = false;

    private readonly Stopwatch _stopwatch = new();

    public double DrawTime { get; private set; }
    public double FlushTime { get; private set; }
    public double DeltaTime => DrawTime + FlushTime;

    public double TotalDraw { get; private set; }
    public double TotalFlush { get; private set; }
    public double TotalDelta => TotalDraw + TotalFlush;

    public int Count { get; private set; }

    public double AverageDraw => Count > 0 ? TotalDraw / Count : 0;
    public double AverageFlush => Count > 0 ? TotalFlush / Count : 0;
    public double AverageDelta => AverageDraw + AverageFlush;


    public void Reset()
    {
        TotalDraw = TotalFlush = DrawTime = FlushTime = 0;
        Count = 0;
    }

    internal void StartDraw()
    {
        if (!Active) return;
        Count++;

        _stopwatch.Restart();
    }

    internal void StartFlush()
    {
        if (!Active) return;
        DrawTime = _stopwatch.Elapsed.TotalSeconds;
        TotalDraw += DrawTime;
        _stopwatch.Restart();
    }

    internal void Stop()
    {
        if (!Active) return;
        FlushTime = _stopwatch.Elapsed.TotalSeconds;
        TotalFlush += FlushTime;
        _stopwatch.Stop();
    }
}