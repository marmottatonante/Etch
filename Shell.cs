using System.Buffers;

namespace Etch;

public static partial class Shell
{
    private static readonly ArrayBufferWriter<byte> _buffer = new();
    private static readonly Stream _output = Console.OpenStandardOutput();

    private static readonly List<(IControl Control, Layout Layout, Rect Cache)> _entries = [];

    private static bool _dirty = false;
    public static void Add(IControl control, Layout layout) { _entries.Add((control, layout, Rect.Empty)); _dirty = true; }
    public static void Clear() { _entries.Clear(); _dirty = true; }
    
    public static Rect Screen => new(Int2.Zero, (Console.WindowWidth, Console.WindowHeight));
    private static Rect _lastScreen = Rect.Empty;

    public static bool AlternateBuffer { set => Console.Write(value ? "\x1b[?1049h" : "\x1b[?1049l"); }
    public static bool Cursor { set => Console.Write(value ? "\x1b[?25h" : "\x1b[?25l"); }

    static Shell() => Platform.EnableAnsi();

    private static void Arrange()
    {
        _lastScreen = Screen;
        for(int i = 0; i < _entries.Count; i++)
        {
            var size = _entries[i].Control.Size;
            var rect = _entries[i].Layout(_lastScreen, size);
            _entries[i] = _entries[i] with { Cache = rect };
        }
    }

    private static void Draw()
    {
        ANSI.Clear(_buffer);
        foreach(var (control, _, cache) in _entries)
            control.Draw(new Region(_buffer, cache));
    }

    private static void Update()
    {
        foreach(var (control, _, cache) in _entries)
            control.Update(new Region(_buffer, cache));
    }

    public static void Render()
    {
        Metrics.StartDraw();

        _buffer.Clear();
        bool hasResized = _lastScreen != Screen;
        if(hasResized || _dirty) { Arrange(); Draw(); _dirty = false; } else Update();

        Metrics.StartFlush();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        Metrics.EndFrame();
    }

    public static void Run(int framerate = 0)
    {
        double target = framerate > 0 ? 1.0 / framerate : 0;
        bool running = true;
        Console.CancelKeyPress += (_, e) => { e.Cancel = true; running = false; };
        while(running)
        {
            Render();
            double remaining = target - Metrics.DeltaTime;
            if(remaining > 0) Thread.Sleep(TimeSpan.FromSeconds(remaining));
        }
    }
}