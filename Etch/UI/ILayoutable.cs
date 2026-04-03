using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch.UI;

public interface ILayoutable
{
    Property<Int2> Position { get; }
    IReadOnlyProperty<Int2> Size { get; }
}