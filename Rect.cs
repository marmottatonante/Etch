namespace Etch;

public readonly record struct Rect(Int2 Position, Int2 Size)
{
    public int Left => Position.X;
    public int Top => Position.Y;
    public int Right => Position.X + Size.X;
    public int Bottom => Position.Y + Size.Y;

    public Int2 Max => Position + Size;
    public static Rect FromMinMax(Int2 min, Int2 max) => new(min, max - min);
    public static Rect Empty => default;

    public bool Contains(Int2 point) =>
        point.X >= Left && point.X < Right &&
        point.Y >= Top  && point.Y < Bottom;

    public Rect? Intersect(Rect other)
    {
        var min = new Int2(Math.Max(Left, other.Left), Math.Max(Top, other.Top));
        var max = new Int2(Math.Min(Right, other.Right), Math.Min(Bottom, other.Bottom));
        return min.X < max.X && min.Y < max.Y ? FromMinMax(min, max) : null;
    }

    public (Rect top, Rect bottom) SplitHorizontal(int y) => (
        FromMinMax(Position, new Int2(Right, Top + y)),
        FromMinMax(new Int2(Left, Top + y), Max)
    );

    public (Rect left, Rect right) SplitVertical(int x) => (
        FromMinMax(Position, new Int2(Left + x, Bottom)),
        FromMinMax(new Int2(Left + x, Top), Max)
    );
}