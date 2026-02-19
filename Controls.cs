namespace Etch;

public interface IControl
{
    Int2 Measure(Int2 available);
    void Render(Context context);
}

public sealed class Center(IControl Child) : IControl
{
    public Int2 Measure(Int2 available) => available;
    public void Render(Context context)
    {
        var size = Child.Measure(context.Bounds.Size);
        var bounds = context.Bounds.Center(size);
        Child.Render(new Context(context.Canvas, bounds));
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

public enum Alignment { Start, Center, End }
public sealed class VerticalStack : IControl
{
    private readonly List<Int2> _measuredSizes = [];
    private readonly List<(IControl Control, Alignment Alignment)> _controls = [];
    public void Add(IControl control, Alignment alignment = Alignment.Start) => _controls.Add((control, alignment));

    public Int2 Measure(Int2 available)
    {
        _measuredSizes.Clear();

        int width = 0;
        int height = 0;

        foreach(var (control, alignment) in _controls)
        {
            int remainingHeight = available.Y - height;
            Int2 size = control.Measure(new(available.X, remainingHeight));
            _measuredSizes.Add(size);

            if(size.X > width) width = size.X;
            height += size.Y;
        }

        return new(width, height);
    }

    public void Render(Context context)
    {
        int y = 0;
        for (int i = 0; i < _controls.Count; i++)
        {
            int x = _controls[i].Alignment switch
            {
                Alignment.Start => context.Bounds.Position.X,
                Alignment.Center => context.Bounds.Position.X + (context.Bounds.Size.X - _measuredSizes[i].X) / 2,
                Alignment.End => context.Bounds.Position.X + context.Bounds.Size.X - _measuredSizes[i].X,
                _ => throw new ArgumentOutOfRangeException()
            };

            var bounds = new Rect(new Int2(x, context.Bounds.Position.Y + y), _measuredSizes[i]);
            _controls[i].Control.Render(new Context(context.Canvas, bounds));
            y += _measuredSizes[i].Y;
        }
    }
}