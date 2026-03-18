using Pith.Geometry;

namespace Etch.UI;

public enum Direction { Vertical, Horizontal }
public enum Alignment { Start, Center, End }

public sealed class Stack(Direction direction, Alignment alignment, int spacing, params IControl[] children) : IControl
{
    private readonly IControl[] _children = children;
    private readonly Direction _direction = direction;
    private readonly Alignment _alignment = alignment;
    private readonly int _spacing = spacing;
    private Int2[] _childSizes = [];

    private int Primary(Int2 size) => _direction == Direction.Vertical ? size.Y : size.X;
    private int Cross(Int2 size) => _direction == Direction.Vertical ? size.X : size.Y;
    private Int2 ToSize(int primary, int cross) => _direction == Direction.Vertical ? new Int2(cross, primary) : new Int2(primary, cross);
    private Int2 ToPosition(int primary, int crossOffset) => _direction == Direction.Vertical ? new Int2(crossOffset, primary) : new Int2(primary, crossOffset);

    private int CrossOffset(int crossSize, int childCross) => _alignment switch
    {
        Alignment.Center => (crossSize - childCross) / 2,
        Alignment.End => crossSize - childCross,
        _ => 0
    };

    public Int2 Measure(Int2 available)
    {
        _childSizes = new Int2[_children.Length];
        var primary = 0;
        var cross = 0;

        for (int i = 0; i < _children.Length; i++)
        {
            _childSizes[i] = _children[i].Measure(available);
            primary += Primary(_childSizes[i]);
            cross = Math.Max(cross, Cross(_childSizes[i]));
        }

        if (_children.Length > 1)
            primary += _spacing * (_children.Length - 1);

        return ToSize(primary, cross);
    }

    public void Arrange(Surface surface)
    {
        var primary = 0;
        var crossSize = Cross(surface.Bounds.Size);

        for (int i = 0; i < _children.Length; i++)
        {
            var childCross = Cross(_childSizes[i]);
            var position = ToPosition(primary, CrossOffset(crossSize, childCross));
            _children[i].Arrange(surface.Slice(new Rect(position, _childSizes[i])));
            primary += Primary(_childSizes[i]) + _spacing;
        }
    }

    public void Render()
    {
        foreach (var child in _children)
            child.Render();
    }
}