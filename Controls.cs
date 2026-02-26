namespace Etch;

public abstract class Control : IMutable
{
    public bool IsInvalid { get; private set; }
    public event Action? Changed;

    protected Control() { Invalidate(); }

    protected void Subscribe(params IMutable[] observables)
    {
        foreach (var observable in observables)
            observable.Changed += Invalidate;
    }

    public void Invalidate()
    {
        IsInvalid = true;
        Changed?.Invoke();
    }

    internal void Render(Region region)
    {
        if (!IsInvalid) return;
        IsInvalid = false;
        Draw(region);
    }

    internal abstract Int2 Measure(Int2 available);
    protected abstract void Draw(Region region);
}

public sealed class Label : Control
{
    public readonly Property<string> Text;

    public Label(string initial) { Text = new(initial);  Subscribe(Text); }
    internal override Int2 Measure(Int2 available) => new(Text.Value.Length, 1);
    protected override void Draw(Region region) => region.Write(Text.Value.AsSpan());
}

public sealed class Image(string[] lines) : Control
{
    public readonly string[] Lines = lines;
    internal override Int2 Measure(Int2 available) =>
        new(Lines.Max(l => l.Length), Lines.Length);
    protected override void Draw(Region region)
    {
        for (int i = 0; i < Lines.Length; i++)
            region.Write((0, i), Lines[i].AsSpan());
    }
}

public sealed class Progress : Control
{
    public readonly double Minimum;
    public readonly Property<double> Current;
    public readonly double Maximum;

    public Progress(double minimum, double maximum)
    {
        Current = new(minimum);
        Minimum = minimum;
        Maximum = maximum;
        Subscribe(Current);
    }
    internal override Int2 Measure(Int2 available) => new(4, 1);
    protected override void Draw(Region region)
    {
        int percentage = (int)((Current.Value - Minimum) / (Maximum - Minimum) * 100);
        Span<char> buffer = stackalloc char[4];
        percentage.TryFormat(buffer, out _, "D3");
        buffer[3] = '%';
        region.Write(buffer);
    }
}

public sealed class Fixed : Control
{
    private readonly Control _child;
    private readonly Int2 _size;

    public Fixed(Control child, Int2 size)
    {
        _child = child;
        _size = size;
        Subscribe(child);
    }

    public T Get<T>() where T : Control => (T)_child;

    internal override Int2 Measure(Int2 available) => _size;
    protected override void Draw(Region region) => _child.Render(region);
}

public sealed class Managed : Control
{
    private readonly Control _child;
    private Region _previousRegion;

    public Managed(Control child)
    {
        _child = child;
        Subscribe(child);
    }

    internal override Int2 Measure(Int2 available) => _child.Measure(available);
    protected override void Draw(Region region)
    {
        if (_previousRegion.Bounds != Rect.Empty)
            _previousRegion.Clear();
        _previousRegion = region;
        _child.Render(region);
    }
}

public sealed class Center : Control
{
    private readonly Control _child;

    public Center(Control child)
    {
        _child = child;
        Subscribe(child);
    }

    public T Get<T>() where T : Control => (T)_child;

    internal override Int2 Measure(Int2 available) => available;
    protected override void Draw(Region region)
    {
        var childSize = _child.Measure(region.Bounds.Size);
        var childRegion = region.Slice(region.Bounds.Center(childSize));
        _child.Render(childRegion);
    }
}

public enum Alignment { Start, Center, End }
public enum Direction { Vertical, Horizontal }
public sealed class Stack : Control
{
    private readonly Control[] _children;
    private readonly Direction _direction;
    private readonly int _spacing;
    private readonly Alignment _alignment;

    public Stack(Direction direction, int spacing, Alignment alignment, params Control[] children)
    {
        _children = children;
        _direction = direction;
        _spacing = spacing;
        _alignment = alignment;
        Subscribe(children);
    }

    private int AlignCross(int childSize, int availableSize) => _alignment switch
    {
        Alignment.Center => (availableSize - childSize) / 2,
        Alignment.End => availableSize - childSize,
        _ => 0
    };

    public T Get<T>(int index) where T : Control => (T)_children[index];

    internal override Int2 Measure(Int2 available)
    {
        int main = 0;
        int cross = 0;
        foreach (var child in _children)
        {
            var size = child.Measure(available);
            var childMain = _direction == Direction.Vertical ? size.Y : size.X;
            var childCross = _direction == Direction.Vertical ? size.X : size.Y;
            main += childMain + _spacing;
            cross = Math.Max(cross, childCross);
        }
        if (_children.Length > 0) main -= _spacing;
        return _direction == Direction.Vertical
            ? new(cross, main)
            : new(main, cross);
    }
    protected override void Draw(Region region)
    {
        int offset = 0;
        foreach (var child in _children)
        {
            var size = child.Measure(region.Bounds.Size);
            var position = _direction == Direction.Vertical
                ? new Int2(AlignCross(size.X, region.Bounds.Size.X), offset)
                : new Int2(offset, AlignCross(size.Y, region.Bounds.Size.Y));
            child.Render(region.Slice(new Rect(position, size)));
            offset += (_direction == Direction.Vertical ? size.Y : size.X) + _spacing;
        }
    }
}

public sealed class Grid : Control
{
    private readonly Stack _root;
    private readonly Control[][] _cells;

    public Grid(Control[][] cells, Int2? cellSize = null)
    {
        _cells = cells;
        var widths = cellSize is null ? ComputeWidths(cells) : null;
        _root = new Stack(Direction.Vertical, 0, Alignment.Start,
            cells.Select((row, r) =>
                new Stack(Direction.Horizontal, 3, Alignment.Start,
                    row.Select((cell, c) =>
                        cellSize is Int2 size
                            ? new Fixed(cell, size)
                            : new Fixed(cell, new Int2(widths![c], 1)))
                    .ToArray()))
            .ToArray());
        Subscribe(_root);
    }

    private static int[] ComputeWidths(Control[][] cells)
    {
        int cols = cells.Max(r => r.Length);
        var widths = new int[cols];
        foreach (var row in cells)
            for (int c = 0; c < row.Length; c++)
                widths[c] = Math.Max(widths[c], row[c].Measure(Int2.Zero).X);
        return widths;
    }

    public T Get<T>(int row, int col) where T : Control => (T)_cells[row][col];

    internal override Int2 Measure(Int2 available) => _root.Measure(available);
    protected override void Draw(Region region) => _root.Render(region);
}