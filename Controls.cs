namespace Etch;

public interface IControl
{
    ReadOnlyProperty<Int2> Size { get; }
    void Render(Region region);
    void Update(Region region);
}

public static class ControlExtensions
{
    public static T With<T>(this T control, Action<T> configure) where T : IControl
    {
        configure(control);
        return control;
    }
}

public sealed class Label : IControl
{
    public readonly Property<string> Text;
    private readonly Property<Int2> _size;
    public ReadOnlyProperty<Int2> Size => _size;

    public Label(string initial)
    {
        Text = new(initial);
        _size = new((initial.Length, 1));

        Text.Changed += t => _size.Set((t.Length, 1));
    }

    public void Update(Region region) { if (Text.Dirty) Render(region); }
    public void Render(Region region) => region.Write(Text.Get());
}

public sealed class Image : IControl
{
    public readonly Property<string[]> Lines;
    private readonly Property<Int2> _size;
    public ReadOnlyProperty<Int2> Size => _size;

    public Image(string[] lines)
    {
        Lines = new(lines);
        _size = new((lines.Max(l => l.Length), lines.Length));

        Lines.Changed += l => _size.Set((l.Max(line => line.Length), l.Length));
    }

    public void Update(Region region) { if (Lines.Dirty) Render(region); }
    public void Render(Region region)
    {
        var lines = Lines.Get();
        for (int i = 0; i < lines.Length; i++)
            region.Write((0, i), lines[i].AsSpan());
    }
}

public sealed class Progress : IControl
{
    public readonly Property<double> Current;
    private readonly Property<int> _percentage;
    public ReadOnlyProperty<Int2> Size { get; } = new((4, 1));

    public Progress(double minimum, double maximum)
    {
        Current = new(minimum);
        _percentage = new(0);

        Current.Changed += c => _percentage.Set((int)((c - minimum) / (maximum - minimum) * 100));
    }

    public void Update(Region region) { if (_percentage.Dirty) Render(region); }
    public void Render(Region region)
    {
        Span<char> buffer = stackalloc char[4];
        _percentage.Get().TryFormat(buffer, out _, "D3");
        buffer[3] = '%';
        region.Write(buffer);
    }
}