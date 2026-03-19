using Keystone.Observables;
using Keystone.Primitives;

namespace Etch;

public abstract class Control
{
    /// <summary>The scene this control is in.</summary>
    public Scene? Scene { get; internal set; }

    /// <summary>The position of this control.</summary>
    public readonly Property<Int2> Position;
    /// <summary>The size of this control.</summary>
    public readonly Property<Int2> Size;

    protected Control()
    {
        Position = Invalidating(Int2.Zero);
        Size = Invalidating(Int2.Zero);
    }

    protected void Invalidate() => Scene?.Invalidate(this);
    protected Property<T> Invalidating<T>(T initial)
    {
        var property = new Property<T>(initial);
        property.Changing += Invalidate;
        return property;
    }

    public abstract void Draw(Canvas canvas);
}