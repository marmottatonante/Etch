namespace Etch;

public abstract class Widget : IControl
{
    private Rect _previousBounds;

    public abstract Int2 Measure(Int2 available);
    protected abstract void Render(Context context);

    void IControl.Render(Context context)
    {
        context.Canvas.Clear(_previousBounds);
        _previousBounds = context.Bounds;
        Render(context);
    }
}

public sealed class Label(string text) : Widget
{
    private readonly string _text = text;
    public override Int2 Measure(Int2 available) => (_text.Length, 1);
    protected override void Render(Context context)
    {
        context.Canvas.Move(context.Bounds.Position);
        context.Canvas.Write(_text.AsSpan());
    }
}

public sealed class Binder(Func<string> source) : Widget
{
    private readonly Func<string> _source = source;
    public override Int2 Measure(Int2 available) => (_source().Length, 1);
    protected override void Render(Context context)
    {
        context.Canvas.Move(context.Bounds.Position);
        context.Canvas.Write(_source().AsSpan());
    }
}

public sealed class Image(string[] lines) : Widget
{
    private readonly string[] _lines = lines;
    public override Int2 Measure(Int2 available) => new(_lines.Max(l => l.Length), _lines.Length);
    protected override void Render(Context context)
    {
        for(int i = 0; i < _lines.Length; i++)
        {
            context.Canvas.Move(context.Bounds.Position + (0, i));
            context.Canvas.Write(_lines[i].AsSpan());
        }
    }
}