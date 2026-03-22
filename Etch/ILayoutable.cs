using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch;

public interface ILayoutable : IRenderable
{
    new Property<Int2> Position { get; }
    new IReadOnlyProperty<Int2> Size { get; }

    IWatchable IRenderable.Position => Position;
    IWatchable IRenderable.Size => Size;
}