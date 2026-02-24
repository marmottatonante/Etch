using System.Buffers;
using System.Diagnostics;

namespace Etch;

public static class Shell
{
    private static readonly ArrayBufferWriter<byte> _buffer = new();
    private static readonly Stream _output = Console.OpenStandardOutput();

    public record struct Entry(IControl Control, Layout Layout, Rect Cache)
    {
        public Entry(IControl Control, Layout Layout) : this(Control, Layout, Rect.Empty) { }
    }
    public static readonly List<Entry> Entries = [];
    public static Rect Screen => new(Int2.Zero, (Console.WindowWidth, Console.WindowHeight));
    private static Rect _lastScreen = Rect.Empty;

    private static readonly Stopwatch _stopwatch = new();

    public static int Frame { get; private set; } = 0;
    public static double DrawTime { get; private set; } = 0;
    public static double FlushTime { get; private set; } = 0;
    public static double DeltaTime => DrawTime + FlushTime;

    public static bool AlternateBuffer { set => Console.Write(value ? "\x1b[?1049h" : "\x1b[?1049l"); }
    public static bool Cursor { set => Console.Write(value ? "\x1b[?25h" : "\x1b[?25l"); }

    static Shell() => Platform.EnableAnsi();

    private static void Arrange()
    {
        _lastScreen = Screen;
        for(int i = 0; i < Entries.Count; i++)
        {
            var size = Entries[i].Control.Size;
            var rect = Entries[i].Layout(_lastScreen, size);
            Entries[i] = Entries[i] with { Cache = rect };
        }
    }

    private static void Draw()
    {
        ANSI.Clear(_buffer);
        foreach(var (control, _, cache) in Entries)
            control.Draw(new Region(_buffer, cache));
    }

    private static void Update()
    {
        foreach(var (control, _, cache) in Entries)
            control.Update(new Region(_buffer, cache));
    }

    public static void Render()
    {
        Frame++;

        _stopwatch.Restart();
        _buffer.Clear();

        bool firstFrame = Frame == 1;
        bool hasResized = !firstFrame && _lastScreen != Screen;
        if(firstFrame || hasResized) { Arrange(); Draw(); } else Update();

        DrawTime = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        FlushTime = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Stop();
    }

    public static void Run(int framerate = 0)
    {
        double target = framerate > 0 ? 1.0 / framerate : 0;
        bool running = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; running = false; };
        while(running)
        {
            Render();
            double remaining = target - DeltaTime;
            if(remaining > 0) Thread.Sleep(TimeSpan.FromSeconds(remaining));
        }
    }
}