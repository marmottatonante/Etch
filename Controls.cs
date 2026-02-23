namespace Etch;

public interface IControl
{
    Int2? Size { get; }
    void Render(Region region);
}

public sealed class Label(string text) : IControl
{
    private readonly string _text = text;
    public Int2? Size { get; } = new(text.Length, 1);
    public void Render(Region region) => region.Write(_text.AsSpan());
}

public sealed class Binder(Func<string> source) : IControl
{
    private readonly Func<string> _source = source;
    public Int2? Size => new(_source().Length, 1);
    public void Render(Region region) => region.Write(_source().AsSpan());
}

public sealed class Image(string[] lines) : IControl
{
    private readonly string[] _lines = lines;
    public Int2? Size { get; } = new(lines.Max(l => l.Length), lines.Length);
    public void Render(Region region)
    {
        for(int i = 0; i < _lines.Length; i++)
            region.Write((0, i), _lines[i].AsSpan());
    }
}

public sealed class Progress(Func<double> current, double maximum) : IControl
{
    private readonly Func<double> _current = current;
    private readonly double _maximum = maximum;

    public Int2? Size { get; } = new(4, 1);
    public void Render(Region region)
    {
        int percentage = (int)(_current() / _maximum * 100);
        Span<char> buffer = stackalloc char[4];
        percentage.TryFormat(buffer, out int written);
        buffer[written++] = '%';
        region.Write(buffer[..written]);
    }
}

public sealed class Center(IControl child) : IControl
{
    public Int2? Size => null;

    public void Render(Region region)
    {
        if(child.Size is not { } size) { child.Render(region); return; }
        
        Int2 offset = new(
            (region.Bounds.Size.X - size.X) / 2,
            (region.Bounds.Size.Y - size.Y) / 2
        );
        child.Render(region.Slice(new Rect(offset, size)));
    }
}

public sealed class Stack(Stack.Axis axis, Stack.Alignment alignment = Stack.Alignment.Start, int spacing = 0, params IControl[] children) : IControl
{
    public enum Axis { Horizontal, Vertical }
    public enum Alignment { Start, Center, End }

    public Int2? Size
    {
        get
        {
            int main = 0, cross = 0;
            foreach(var child in children)
            {
                if(child.Size is not { } size) return null;
                if(axis == Axis.Vertical)
                {
                    main += size.Y;
                    cross = Math.Max(cross, size.X);
                }
                else
                {
                    main += size.X;
                    cross = Math.Max(cross, size.Y);
                }
            }
            int totalSpacing = spacing * (children.Length - 1);
            return axis == Axis.Vertical
                ? new(cross, main + totalSpacing)
                : new(main + totalSpacing, cross);
        }
    }

    public void Render(Region region)
    {
        int staticTotal = 0;
        int elasticCount = 0;
        foreach(var child in children)
            if(child.Size is { } size)
                staticTotal += axis == Axis.Vertical ? size.Y : size.X;
            else
                elasticCount++;

        int totalSpacing = spacing * (children.Length - 1);
        int totalSpace = axis == Axis.Vertical ? region.Bounds.Size.Y : region.Bounds.Size.X;
        int elasticSpace = elasticCount > 0 ? (totalSpace - staticTotal - totalSpacing) / elasticCount : 0;

        int offset = 0;
        foreach(var child in children)
        {
            Int2 childSize = child.Size ?? (
                axis == Axis.Vertical
                    ? new(region.Bounds.Size.X, elasticSpace)
                    : new(elasticSpace, region.Bounds.Size.Y)
            );

            int crossSize = axis == Axis.Vertical ? childSize.X : childSize.Y;
            int crossTotal = axis == Axis.Vertical ? region.Bounds.Size.X : region.Bounds.Size.Y;
            int crossOffset = alignment switch
            {
                Alignment.Center => (crossTotal - crossSize) / 2,
                Alignment.End => crossTotal - crossSize,
                _ => 0
            };

            Int2 slicePosition = axis == Axis.Vertical
                ? new(crossOffset, offset)
                : new(offset, crossOffset);

            child.Render(region.Slice(new Rect(slicePosition, childSize)));
            offset += (axis == Axis.Vertical ? childSize.Y : childSize.X) + spacing;
        }
    }
}