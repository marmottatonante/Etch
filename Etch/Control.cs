using Pith.Geometry;
using Pith.Reactive;

namespace Etch;

public abstract class Control : Renderable
{
    private Surface? _surface;

    public Control()
    {
        Parent.Changing += () => { if (Parent.Value is not null) Size.Changed -= Parent.Value.Invalidate; };
        Parent.Changed += () => { if (Parent.Value is not null) Size.Changed += Parent.Value.Invalidate; };
    }

    public sealed override Int2 Measure(Int2 available) => Size.Value;
    public sealed override void Arrange(Surface surface)
    {
        if (_surface.HasValue && _surface.Value.Bounds == surface.Bounds) return;
        _surface?.Clear();
        _surface = surface;
        Invalidate();
    }
    public sealed override void Render()
    {
        if (!_surface.HasValue) throw new InvalidOperationException("Control has not been arranged yet.");
        if (!Invalid) return;
        Invalid = false;
        Render(_surface.Value);
    }

    public abstract Property<Int2> Size { get; }
    public abstract void Render(Surface surface);
}