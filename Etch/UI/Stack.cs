using Pith.Geometry;

namespace Etch.UI;

public enum Direction { Vertical, Horizontal }
public enum Alignment { Start, Center, End }

public class Stack : Layout
{
    private readonly Direction _direction;
    private readonly Alignment _alignment;
    private readonly int _spacing;

    public Stack(Direction direction,
                 Alignment alignment, 
                 int spacing, 
                 params Renderable[] children) : base(children)
    {
        _direction = direction;
        _alignment = alignment;
        _spacing = spacing;
    }

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

    protected override Int2 ComputeSize(Int2 available, Int2[] childSizes)
    {
        var primary = childSizes.Sum(s => Primary(s)) + _spacing * (childSizes.Length - 1);
        var cross = childSizes.Max(s => Cross(s));
        return ToSize(primary, cross);
    }

    protected override Surface[] ComputeSurfaces(Surface surface, Int2[] childSizes)
    {
        var surfaces = new Surface[childSizes.Length];
        var primary = 0;
        var crossSize = Cross(surface.Bounds.Size);

        for (int i = 0; i < childSizes.Length; i++)
        {
            var childCross = Cross(childSizes[i]);
            var position = ToPosition(primary, CrossOffset(crossSize, childCross));
            surfaces[i] = surface.Slice(new Rect(position, childSizes[i]));
            primary += Primary(childSizes[i]) + _spacing;
        }

        return surfaces;
    }
}