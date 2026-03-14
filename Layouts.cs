namespace Etch;

public sealed class Center : IControl
{
    public readonly IControl Child;
    private bool _layoutDirty;

    public ReadOnlyProperty<Int2> Size { get; } = new((-1, -1));

    public Center(IControl child)
    {
        Child = child;
        _layoutDirty = true;
        Child.Size.Changed += _ => _layoutDirty = true;
    }

    public void Render(Region region)
    {
        _layoutDirty = false;
        Child.Render(region.Slice(region.Bounds.Center(Child.Size.Peek())));
    }

    public void Update(Region region)
    {
        var rect = region.Bounds.Center(Child.Size.Peek());
        if (_layoutDirty)
        {
            _layoutDirty = false;
            Child.Render(region.Slice(rect));
        }
        else Child.Update(region.Slice(rect));
    }
}

public enum Direction { Vertical, Horizontal }
public enum Alignment { Start, Center, End }
public sealed class Stack(Direction direction, int spacing, Alignment alignment, params IControl[] children) : IControl
{
    private readonly Rect[] _rects = new Rect[children.Length];

    private int AlignCross(int childSize, int availableSize) => alignment switch
    {
        Alignment.Center => (availableSize - childSize) / 2,
        Alignment.End => availableSize - childSize,
        _ => 0
    };

    public Int2 Measure(Int2 available)
    {
        bool resized = false;
        for (int i = 0; i < children.Length; i++)
        {
            var childSize = children[i].Measure(available);
            if (childSize == _rects[i].Size) continue;
            _rects[i] = new Rect(_rects[i].Position, childSize);
            resized = true;
        }
        if (resized) Recompute(available);

        int main = 0;
        int cross = 0;
        foreach (var rect in _rects)
        {
            main += (direction == Direction.Vertical ? rect.Size.Y : rect.Size.X) + spacing;
            cross = Math.Max(cross, direction == Direction.Vertical ? rect.Size.X : rect.Size.Y);
        }
        if (_rects.Length > 0) main -= spacing;
        return direction == Direction.Vertical ? new(cross, main) : new(main, cross);
    }

    private void Recompute(Int2 available)
    {
        int offset = 0;
        for (int i = 0; i < _rects.Length; i++)
        {
            var size = _rects[i].Size;
            var position = direction == Direction.Vertical
                ? new Int2(AlignCross(size.X, available.X), offset)
                : new Int2(offset, AlignCross(size.Y, available.Y));
            _rects[i] = new Rect(position, size);
            offset += (direction == Direction.Vertical ? size.Y : size.X) + spacing;
        }
    }

    public void Render(Region region)
    {
        for (int i = 0; i < children.Length; i++)
            children[i].Render(region.Slice(_rects[i]));
    }

    public void Update(Region region)
    {
        for (int i = 0; i < children.Length; i++)
            children[i].Update(region.Slice(_rects[i]));
    }
}

public sealed class Grid(IControl[][] cells, Int2? cellSize = null) : IControl
{
    private readonly Stack _root = Build(cells, cellSize);

    private static Stack Build(IControl[][] cells, Int2? cellSize)
    {
        var widths = cellSize is null ? ComputeWidths(cells) : null;
        return new Stack(Direction.Vertical, 0, Alignment.Start,
            cells.Select((row, r) =>
                new Stack(Direction.Horizontal, 3, Alignment.Start,
                    row.Select((cell, c) =>
                        new Fixed(cell, cellSize ?? new(widths![c], 1)))
                    .ToArray()))
            .ToArray());
    }

    private static int[] ComputeWidths(IControl[][] cells)
    {
        int cols = cells.Max(r => r.Length);
        var widths = new int[cols];
        for (int r = 0; r < cells.Length; r++)
            for (int c = 0; c < cells[r].Length; c++)
            {
                cells[r][c].Measure(Int2.Zero, out var size);
                widths[c] = Math.Max(widths[c], size.X);
            }
        return widths;
    }

    public bool Measure(Int2 available, out Int2 size) => _root.Measure(available, out size);
    public void Render(Region region) => _root.Render(region);
}