namespace Etch;

public interface ILayout
{
    Rect Arrange(Rect available);
}

/*
public sealed class Center(IControl child) : ILayout
{
    private readonly IControl _child = child;
}

public enum Alignment { Start, Center, End }
public sealed class VerticalStack(List<(IControl, Alignment)> controls) : ILayout
{
    private readonly List<(IControl Control, Alignment Alignment)> _controls = controls;
    private readonly List<Int2> _measuredSizes = [];
    public Int2 Arrange(Int2 available)
    {
        _measuredSizes.Clear();

        int width = 0;
        int height = 0;

        foreach(var (control, alignment) in _controls)
        {
            int remainingHeight = available.Y - height;
            Int2  size = control.Arrange(new(available.X, remainingHeight));
            _measuredSizes.Add(size);

            if (size.X > width) width = size.X;
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
}*/