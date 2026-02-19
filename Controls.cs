namespace Etch;

public interface IControl
{
    Int2 Measure(Int2 available);
    void Render(Context context);
}

public sealed record class Panel : IControl
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

public sealed record class Center(IControl Child) : IControl
{
    public Int2 Measure(Int2 available) => available;
    public void Render(Context context)
    {
        var size = Child.Measure(context.Bounds.Size);
        var bounds = context.Bounds.Center(size);
        Child.Render(new Context(context.Canvas, bounds));
    }
}