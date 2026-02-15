namespace Etch;

public readonly record struct Int2(int X, int Y)
{
    public static Int2 Zero => new(0, 0);
    public static Int2 One => new(1, 1);

    public static Int2 operator +(Int2 a, Int2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Int2 operator -(Int2 a, Int2 b) => new(a.X - b.X, a.Y - b.Y);
    
    public static Int2 operator *(Int2 a, int scalar) => new(a.X * scalar, a.Y * scalar);

    public static implicit operator Int2((int X, int Y) t) => new(t.X, t.Y);
    public static implicit operator (int, int)(Int2 int2) => new(int2.X, int2.Y);
}

public readonly record struct Rect(Int2 Position, Int2 Size)
{
    public int Left => Position.X;
    public int Top => Position.Y;
    public int Right => Position.X + Size.X;
    public int Bottom => Position.Y + Size.Y;

    public bool Contains(Int2 point) =>
        point.X >= Left && point.X < Right &&
        point.Y >= Top && point.Y < Bottom;

    public Rect Intersect(Rect other)
    {
        int x1 = Math.Max(Left, other.Left);
        int x2 = Math.Min(Right, other.Right);
        int y1 = Math.Max(Top, other.Top);
        int y2 = Math.Min(Bottom, other.Bottom);

        if (x2 >= x1 && y2 >= y1)
            return new Rect(new Int2(x1, y1), new Int2(x2 - x1, y2 - y1));

        return new Rect(Int2.Zero, Int2.Zero);
    }
}