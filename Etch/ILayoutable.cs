using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch;

public interface ILayoutable
{
    Property<Int2> Position { get; }
    IReadOnlyProperty<Int2> Size { get; }
}