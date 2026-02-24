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

    public static class Settings
    {
        public static bool AlternateBuffer { set => Console.Write(value ? "\x1b[?1049h" : "\x1b[?1049l"); }
        public static bool ArrangeOnResize { get; set; }
        public static bool ForceDraw { get; set; }
    }

    private static readonly Stopwatch _stopwatch = new();

    public static int Frame { get; private set; } = 0;
    public static double DrawTime { get; private set; } = 0;
    public static double FlushTime { get; private set; } = 0;
    public static double RenderTime => DrawTime + FlushTime;

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
        bool requiresArrange = firstFrame || (hasResized && Settings.ArrangeOnResize);

        if(requiresArrange) Arrange();
        if(requiresArrange || Settings.ForceDraw) Draw(); else Update();
        if(Settings.ForceDraw) Settings.ForceDraw = false;

        DrawTime = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Restart();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        FlushTime = _stopwatch.Elapsed.TotalSeconds;
        _stopwatch.Stop();
    }
}