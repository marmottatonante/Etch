namespace Etch.Core;

public interface IControl
{
    Int2 Measure(Int2 available);
    void Arrange(Surface surface);
    void Render();
}

public abstract class Control : IControl
{
    private Surface? _surface;
    private bool _invalid = true;

    protected Property<T> Invalidating<T>(T initial)
    {
        var property = new Property<T>(initial);
        property.Changed += Invalidate;
        return property;
    }

    protected void Invalidate() => _invalid = true;

    void IControl.Arrange(Surface surface)
    {
        if (_surface.HasValue && _surface.Value.Bounds == surface.Bounds) return;

        _surface?.Clear();
        _surface = surface;
        Invalidate();
    }
    void IControl.Render()
    {
        if (!_surface.HasValue) throw new InvalidOperationException("Control has not been arranged yet.");
        if (!_invalid) return;

        _invalid = false;
        Render(_surface.Value);
    }

    public T With<T>(Action<T> configure) where T : Control
    {
        configure((T)this);
        return (T)this;
    }

    public abstract Int2 Measure(Int2 available);
    public abstract void Render(Surface surface);
}