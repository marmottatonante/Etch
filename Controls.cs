namespace Etch;

public interface IControl
{   
    Int2 Size { get; }
    void Draw(Region region);
    void Update(Region region);
}

public sealed class Label(string text) : IControl
{
    public readonly string Text = text;
    public Int2 Size { get; } = new(text.Length, 1);
    public void Draw(Region region) => region.Write(Text.AsSpan());
    public void Update(Region region) { }
}

public sealed class Image(string[] content) : IControl
{
    public readonly string[] Content = content;
    public Int2 Size { get; } = new(content.Max(l => l.Length), content.Length);
    public void Draw(Region region)
    {
        for(int i = 0; i < Content.Length; i++)
            region.Write((0, i), Content[i].AsSpan());
    }
    public void Update(Region region) { }
}

public sealed class Binder(Func<string> source, int maxLength) : IControl
{
    public readonly Func<string> Source = source;
    public Int2 Size { get; } = new(maxLength, 1);
    public void Draw(Region region) => region.Write(Source().AsSpan());
    public void Update(Region region) => Draw(region);
}

public sealed class Progress(Func<double> current, double maximum) : IControl
{
    public readonly Func<double> Current = current;
    public readonly double Maximum = maximum;
    public Int2 Size { get; } = new(4, 1);
    public void Draw(Region region)
    {
        int percentage = (int)(Current() / Maximum * 100);
        Span<char> buffer = stackalloc char[4];
        percentage.TryFormat(buffer, out _, "D3");
        buffer[3] = '%';
        region.Write(buffer);
    }
    public void Update(Region region) => Draw(region);
}

public sealed class Table : IControl
{
    private readonly string[][] _rows;
    private readonly int[] _widths;
    private readonly int _totalWidth;

    public Table(string[][] rows)
    {
        _rows = rows;
        _widths = ComputeWidths(rows);
        _totalWidth = _widths.Sum() + (_widths.Length - 1) * 3;
    }

    private static int[] ComputeWidths(string[][] rows)
    {
        int cols = rows.Max(r => r.Length);
        var widths = new int[cols];
        foreach (var row in rows)
            for (int c = 0; c < row.Length; c++)
                widths[c] = Math.Max(widths[c], row[c].Length);
        return widths;
    }

    public Int2 Size => new(_totalWidth, _rows.Length);

    private void RenderRow(Region region, int y, string[] cells)
    {
        Span<char> buffer = stackalloc char[_totalWidth];
        buffer.Fill(' ');

        int pos = 0;
        for (int c = 0; c < _widths.Length; c++)
        {
            if (c > 0)
            {
                buffer[pos++] = ' ';
                buffer[pos++] = '|';
                buffer[pos++] = ' ';
            }
            string cell = c < cells.Length ? cells[c] : "";
            if (c == 0)
            {
                cell.AsSpan().CopyTo(buffer[pos..]);
                pos += _widths[c];
            }
            else
            {
                int padding = _widths[c] - cell.Length;
                cell.AsSpan().CopyTo(buffer[(pos + padding)..]);
                pos += _widths[c];
            }
        }

        region.Write((0, y), buffer);
    }

    public void Draw(Region region)
    {
        for (int i = 0; i < _rows.Length; i++)
            RenderRow(region, i, _rows[i]);
    }

    public void Update(Region region) { }
}