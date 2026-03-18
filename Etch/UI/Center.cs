using Etch.Core;

namespace Etch.UI;

public sealed class Center(IControl child) : IControl
{
    private readonly IControl _child = child;
    private Int2 _childSize;

    public Int2 Measure(Int2 available)
    {
        _childSize = _child.Measure(available);
        return available;
    }

    public void Arrange(Surface surface)
    {
        var offset = new Int2(
            (surface.Bounds.Size.X - _childSize.X) / 2,
            (surface.Bounds.Size.Y - _childSize.Y) / 2
        );
        _child.Arrange(surface.Slice(new Rect(offset, _childSize)));
    }

    public void Render() => _child.Render();
}