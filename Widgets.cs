namespace Etch;

public abstract record class Widget : IControl
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

public sealed record class Label(string Text) : Widget
{
    public override Int2 Measure(Int2 available) => (Text.Length, 1);
    protected override void Render(Context context)
    {
        context.Canvas.Move(context.Bounds.Position);
        context.Canvas.Write(Text.AsSpan());
    }
}

public sealed record class Binder(Func<string> Source) : Widget
{
    public override Int2 Measure(Int2 available) => (Source().Length, 1);
    protected override void Render(Context context)
    {
        context.Canvas.Move(context.Bounds.Position);
        context.Canvas.Write(Source().AsSpan());
    }
}

public sealed record class Image(string[] Lines) : Widget
{
    public override Int2 Measure(Int2 available) => new(Lines.Max(l => l.Length), Lines.Length);
    protected override void Render(Context context)
    {
        for(int i = 0; i < Lines.Length; i++)
        {
            context.Canvas.Move(context.Bounds.Position + (0, i));
            context.Canvas.Write(Lines[i].AsSpan());
        }
    }
}