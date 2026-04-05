using Etch.Drawing;
using Keystone;

namespace Etch.UI;

public sealed class Container : ILayoutable
{
    public ILayoutable[] Children { get; }

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }

    public Container(params ILayoutable[] children)
    {
        Position = new(Int2.Zero);
        Children = children;

        Size = new Property<Int2>(ComputeSize,
            children.SelectMany(child => new[] { child.Position, child.Size }).ToArray());
    }

    private Int2 ComputeSize()
    {
        int maxX = 0;
        int maxY = 0;

        foreach (var child in Children)
        {
            int childRight = child.Position.Value.X + child.Size.Value.X;
            int childBottom = child.Position.Value.Y + child.Size.Value.Y;

            maxX = Math.Max(maxX, childRight);
            maxY = Math.Max(maxY, childBottom);
        }

        return new Int2(maxX, maxY);
    }
}
