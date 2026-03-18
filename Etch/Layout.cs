using Keystone.Primitives;

namespace Etch;

public abstract class Layout : Renderable
{
    private Int2 _cachedSize;
    private Int2[] _childSizes = [];

    protected Layout(params Renderable[] children)
    {
        foreach (var child in children)
            Add(child);
    }

    public sealed override Int2 Measure(Int2 available)
    {
        if (!Invalid) return _cachedSize;

        _childSizes = new Int2[Children.Count];
        for (int i = 0; i < Children.Count; i++)
            _childSizes[i] = Children[i].Measure(available);

        _cachedSize = ComputeSize(available, _childSizes);
        Invalid = false;
        return _cachedSize;
    }

    public sealed override void Arrange(Surface surface)
    {
        var surfaces = ComputeSurfaces(surface, _childSizes);
        for (int i = 0; i < Children.Count; i++)
            Children[i].Arrange(surfaces[i]);
    }

    public sealed override void Render()
    {
        foreach (var child in Children)
            child.Render();
    }

    protected abstract Int2 ComputeSize(Int2 available, Int2[] childSizes);
    protected abstract Surface[] ComputeSurfaces(Surface surface, Int2[] childSizes);
}