using Pith.Geometry;

namespace Etch.UI;

public class Grid : IControl
{
    private readonly Stack _root;

    public Grid(int columnSpacing, int rowSpacing, params IControl[][] rows)
    {
        var columnCount = rows.Max(r => r.Length);
        var columns = Enumerable.Range(0, columnCount)
            .Select(col => rows
                .Where(row => col < row.Length)
                .Select(row => row[col])
                .ToArray())
            .ToArray();

        _root = new Stack(Direction.Horizontal, Alignment.Start, columnSpacing,
            columns.Select(column => new Stack(Direction.Vertical, Alignment.Start, rowSpacing, column))
            .ToArray());
    }

    public Int2 Measure(Int2 available) => _root.Measure(available);
    public void Arrange(Surface surface) => _root.Arrange(surface);
    public void Render() => _root.Render();
}