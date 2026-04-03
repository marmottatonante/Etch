using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch.UI;

public abstract class Widget : IRenderable, ILayoutable
{
    public abstract Property<Int2> Position { get; }
    public abstract IReadOnlyProperty<Int2> Size { get; }
    public abstract IWatchable Content { get; }

    IWatchable IRenderable.Position => Position;
    IWatchable IRenderable.Size => Size;

    public abstract void Render(Canvas canvas);
    public virtual void Clear(Canvas canvas)
    {
        for (int i = 0; i < Size.Value.Y; i++)
            canvas.Move((Position.Value.X, Position.Value.Y + i))
                  .Blank(Size.Value.X);
    }
}
