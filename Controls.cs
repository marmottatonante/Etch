namespace Etch;

public interface IControl
{   
    Int2 Size { get; }
    void Draw(Region region);
    void Update(Region region) { }
}

public sealed class Label(string text) : IControl
{
    public readonly string Text = text;
    public Int2 Size { get; } = new(text.Length, 1);
    public void Draw(Region region) => region.Write(Text.AsSpan());
}

public sealed class Image(string[] content) : IControl
{
    public readonly string[] Content = content;
    public Int2 Size { get; } = new(content.Max(l => l.Length), content.Length);
    public void Draw(Region region)
    {
        for(int i = 0; i < Content.Length; i++)
            region.Write((0, i), Content[i].AsSpan());
    }
}

public sealed class Binder(Func<string> source, int maxLength)
{
    public readonly Func<string> Source = source;
    public Int2 Size { get; } = new(maxLength, 1);
    public void Draw(Region region) => region.Write(Source().AsSpan());
    public void Update(Region region) => Draw(region);
}

public sealed class Progress(Func<double> current, double maximum) : IControl
{
    public readonly Func<double> Current = current;
    public readonly double Maximum = maximum;
    public Int2 Size { get; } = new(4, 1);
    public void Draw(Region region)
    {
        int percentage = (int)(Current() / Maximum * 100);
        Span<char> buffer = stackalloc char[4];
        percentage.TryFormat(buffer, out int written);
        buffer[written++] = '%';
        region.Write(buffer[..written]);
    }
    public void Update(Region region) => Draw(region);
}