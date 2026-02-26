namespace Etch;

public abstract class Control : IMutable
{
    public bool IsInvalid { get; private set; }
    public event Action? Changed;

    protected Control() { Invalidate(); }

    protected void Subscribe(params IMutable[] observables)
    {
        foreach (var observable in observables)
            observable.Changed += Invalidate;
    }

    public void Invalidate()
    {
        IsInvalid = true;
        Changed?.Invoke();
    }

    internal void Render(Region region)
    {
        if (!IsInvalid) return;
        IsInvalid = false;
        Draw(region);
    }

    internal abstract Int2 Measure(Int2 available);
    protected abstract void Draw(Region region);
}

public sealed class Label : Control
{
    public readonly Property<string> Text;

    public Label(string initial) { Text = new(initial);  Subscribe(Text); }
    internal override Int2 Measure(Int2 available) => new(Text.Value.Length, 1);
    protected override void Draw(Region region) => region.Write(Text.Value.AsSpan());
}

public sealed class Center : Control
{
    private readonly Control _child;

    public Center(Control child)
    {
        _child = child;
        Subscribe(child);
    }
    internal override Int2 Measure(Int2 available) => available;
    protected override void Draw(Region region)
    {
        var childSize = _child.Measure(region.Bounds.Size);
        var childRegion = region.Slice(region.Bounds.Center(childSize));
        _child.Render(childRegion);
    }
}