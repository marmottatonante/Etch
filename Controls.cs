namespace Etch;

public interface IControl
{
    Int2 Measure(Int2 available);
    void Render(Context context);
}

public sealed record class Label(string Text) : IControl
{
    public Int2 Measure(Int2 available) => (Text.Length, 1);
    public void Render(Context context)
    {
        context.Canvas.Move(context.Bounds.Position);
        context.Canvas.Write(Text.AsSpan());
    }
}