namespace Etch;

public sealed class Table(string[][] rows) : IControl
{
    private readonly int[] _widths = ComputeWidths(rows);

    private static int[] ComputeWidths(string[][] rows)
    {
        int cols = rows.Max(r => r.Length);
        var widths = new int[cols];
        foreach (var row in rows)
            for (int c = 0; c < row.Length; c++)
                widths[c] = Math.Max(widths[c], row[c].Length);
        return widths;
    }

    public Int2 Size
    {
        get
        {
            int width = _widths.Sum() + (_widths.Length - 1) * 3;
            return new(width, rows.Length);
        }
    }

    private void RenderRow(Region region, int y, string[] cells)
    {
        var sb = new System.Text.StringBuilder();
        for (int c = 0; c < _widths.Length; c++)
        {
            if (c > 0) sb.Append(" | ");
            string cell = c < cells.Length ? cells[c] : "";
            sb.Append(c == 0 ? cell.PadRight(_widths[c]) : cell.PadLeft(_widths[c]));
        }
        region.Write((0, y), sb.ToString());
    }

    public void Draw(Region region)
    {
        for (int i = 0; i < rows.Length; i++)
            RenderRow(region, i, rows[i]);
    }

    public void Update(Region region) { }
}