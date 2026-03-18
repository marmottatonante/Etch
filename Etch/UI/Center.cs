using Pith.Geometry;

namespace Etch.UI;

public sealed class Center(Renderable child) : Layout(child)
{
    protected override Int2 ComputeSize(Int2 available, Int2[] childSizes) => available;
    protected override Surface[] ComputeSurfaces(Surface surface, Int2[] childSizes) =>
    [
        surface.Slice(new Rect(
            new Int2(
                (surface.Bounds.Size.X - childSizes[0].X) / 2,
                (surface.Bounds.Size.Y - childSizes[0].Y) / 2),
            childSizes[0]))
    ];
}