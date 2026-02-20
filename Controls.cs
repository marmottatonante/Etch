namespace Etch;

public interface IControl
{
    Int2 Size { get; }
    void Render(Region region);
}

public sealed class Label(string text) : IControl
{
    private readonly string _text = text;
    public Int2 Size => new(_text.Length, 1);
    public void Render(Region region) => region.Move((0, 0)).Write(_text.AsSpan());
}

public sealed class Binder(Func<string> source) : IControl
{
    private readonly Func<string> _source = source;
    public Int2 Size => new(_source().Length, 1);
    public void Render(Region region) => region.Move((0, 0)).Write(_source().AsSpan());
}

public sealed class Image(string[] lines) : IControl
{
    private readonly string[] _lines = lines;
    public Int2 Size => new(_lines.Max(l => l.Length), _lines.Length);
    public void Render(Region region)
    {
        for(int i = 0; i < _lines.Length; i++)
        {
            region.Move((0, i));
            region.Write(_lines[i].AsSpan());
        }
    }
}