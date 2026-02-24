using System.Buffers;
using System.Diagnostics;

namespace Etch;

public static class Shell
{
    private static readonly ArrayBufferWriter<byte> _buffer = new();
    private static readonly Stream _output = Console.OpenStandardOutput();
    private static bool _needsDraw = true;

    public record struct Entry(IControl Control, Func<Rect, Rect> Layout, Rect Cache)
    {
        public Entry(IControl control, Func<Rect, Rect> layout) : this(control, layout, Rect.Empty) { }
    }
    public static readonly List<Entry> Entries = [];
    public static Rect Screen => new(Int2.Zero, (Console.WindowWidth, Console.WindowHeight));
    private static Rect _lastScreen = Rect.Empty;

    public static class Settings
    {
        public static bool AlternateBuffer { set => Console.Write(value ? "\x1b[?1049h" : "\x1b[?1049l"); }
        public static bool ArrangeOnResize { get; set; }
    }

    private static readonly Stopwatch _stopwatch = new();

    public static double RenderTime { get; private set; }
    public static double FlushTime { get; private set; }
    public static double DeltaTime => RenderTime + FlushTime;

    static Shell() => Platform.EnableAnsi();

    private static void Arrange()
    {
        _lastScreen = Screen;
        for(int i = 0; i < Entries.Count; i++)
        {
            var rect = Entries[i].Layout(_lastScreen);
            Entries[i] = Entries[i] with { Cache = rect };
        }
    }

    private static void Draw()
    {
        ANSI.Clear(_buffer);
        foreach(var (control, _, cache) in Entries)
            control.Draw(new Region(_buffer, cache));
        _needsDraw = false;
    }

    private static void Update()
    {
        if(Entries is null) return;
        foreach(var (control, _, cache) in Entries)
            control.Update(new Region(_buffer, cache));
    }

    public static void Render()
    {
        _stopwatch.Restart();
        _buffer.Clear();
        if(_lastScreen != Screen && Settings.ArrangeOnResize) Arrange();
        if(_needsDraw) Draw(); else Update();
        RenderTime = _stopwatch.Elapsed.TotalSeconds;

        _stopwatch.Restart();
        _output.Write(_buffer.WrittenSpan);
        _output.Flush();
        FlushTime = _stopwatch.Elapsed.TotalSeconds;
    }
}