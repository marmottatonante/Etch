using Keystone;

namespace Etch.Drawing;

public interface ILayoutable
{
    Property<Int2> Position { get; }
    IReadOnlyProperty<Int2> Size { get; }
}