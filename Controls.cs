using Microsoft.VisualBasic;

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

public sealed record class Binder(Func<string> Source) : IControl
{
    public Int2 Measure(Int2 available) => (Source().Length, 1);
    public void Render(Context context)
    {
        context.Canvas.Move(context.Bounds.Position);
        context.Canvas.Write(Source().AsSpan());
    }
}

public sealed class Panel : IControl
{
    private readonly List<(IControl, Int2)> _controls = [];
    public void Add(IControl control, Int2 at) => _controls.Add((control, at));

    public Int2 Measure(Int2 available) => available;
    public void Render(Context context)
    {
        foreach (var (control, position) in _controls)
        {
            var size = control.Measure(context.Bounds.Size);
            var bounds = new Rect(context.Bounds.Position + position, size);
            control.Render(new Context(context.Canvas, bounds));
        }
    }
}