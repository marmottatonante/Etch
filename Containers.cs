namespace Etch;

public sealed class Managed(IControl child) : IControl
{
    public readonly IControl Child = child;
    private Region _previousRegion;

    public bool Measure(Int2 available, out Int2 size) => Child.Measure(available, out size);
    public void Render(Region region)
    {
        if (_previousRegion.Bounds != Rect.Empty)
            _previousRegion.Clear();
        _previousRegion = region;
        Child.Render(region);
    }
}

public sealed class Fixed(IControl child, Int2 size) : IControl
{
    public Property<Int2> Size { get; } = new(size);
    public bool Measure(Int2 available, out Int2 size) => Size.TryGet(out size);
    public void Render(Region region) => child.Render(region);
}