using Keystone.Collections;
using Keystone.Primitives;
using Keystone.Observables;

namespace Etch;

public abstract class Renderable : Node<Renderable>
{
    public bool Invalid { get; protected set; } = true;
    internal void Invalidate() => Invalid = true;
    protected Property<T> Invalidating<T>(T initial)
    {
        var property = new Property<T>(initial);
        property.Changed += Invalidate;
        return property;
    }

    public abstract Int2 Measure(Int2 available);
    public abstract void Arrange(Surface surface);
    public abstract void Render();
}
