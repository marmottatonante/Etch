namespace Etch;

public interface IControl
{
    Int2? Size { get; }
    void Render(Region region);
}

public sealed class Label(string text) : IControl
{
    private readonly string _text = text;
    public Int2? Size => new(_text.Length, 1);
    public void Render(Region region) => region.Write(_text.AsSpan());
}

public sealed class Binder(Func<string> source, int? length = null) : IControl
{
    private readonly Func<string> _source = source;
    public Int2? Size { get; } = length is not null ? (length.Value, 1) : null;
    public void Render(Region region) => region.Write(_source().AsSpan());
}

public sealed class Image(string[] lines) : IControl
{
    private readonly string[] _lines = lines;
    public Int2? Size => new(_lines.Max(l => l.Length), _lines.Length);
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
        double currentPercentage = _current() / _maximum * 100;
        region.Write($"{(int)currentPercentage}%");
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

public sealed class VerticalStack(int spacing = 0, params IControl[] children) : IControl
{
    public Int2? Size
    {
        get
        {
            int width = 0, height = 0;
            foreach(var child in children)
            {
                if(child.Size is not { } size) return null;
                width = Math.Max(width, size.X);
                height += size.Y;
            }
            return new(width, height + spacing * (children.Length - 1));
        }
    }

    public void Render(Region region)
    {
        int staticTotal = 0;
        int elasticCount = 0;
        foreach(var child in children)
            if(child.Size is { } size) staticTotal += size.Y;
            else elasticCount++;

        int elasticSpace = elasticCount > 0 
            ? (region.Bounds.Size.Y - staticTotal - spacing * (children.Length - 1)) / elasticCount 
            : 0;

        int offset = 0;
        foreach(var child in children)
        {
            int childHeight = child.Size?.Y ?? elasticSpace;
            child.Render(region.Slice(new Rect(new Int2(0, offset), new Int2(region.Bounds.Size.X, childHeight))));
            offset += childHeight + spacing;
        }
    }
}

public sealed class HorizontalStack(int spacing = 0, params IControl[] children) : IControl
{
    public Int2? Size
    {
        get
        {
            int width = 0, height = 0;
            foreach(var child in children)
            {
                if(child.Size is not { } size) return null;
                height = Math.Max(height, size.Y);
                width += size.X;
            }
            return new(width + spacing * (children.Length - 1), height);
        }
    }

    public void Render(Region region)
    {
        int staticTotal = 0;
        int elasticCount = 0;
        foreach(var child in children)
            if(child.Size is { } size) staticTotal += size.X;
            else elasticCount++;

        int elasticSpace = elasticCount > 0 
            ? (region.Bounds.Size.X - staticTotal - spacing * (children.Length - 1)) / elasticCount 
            : 0;

        int offset = 0;
        foreach(var child in children)
        {
            int childWidth = child.Size?.X ?? elasticSpace;
            child.Render(region.Slice(new Rect(new Int2(offset, 0), new Int2(childWidth, region.Bounds.Size.Y))));
            offset += childWidth + spacing;
        }
    }
}