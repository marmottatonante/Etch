namespace Etch;

public delegate Rect Layout(Rect available, Int2 size);
public static class Layouts
{
    public static Layout Absolute(Int2 position) =>
        (available, size) => new Rect(position, size);

    public static Layout TopLeft =>
        (available, size) => new Rect(available.Position, size);

    public static Layout TopCenter =>
        (available, size) => new Rect(new Int2(available.Position.X + (available.Size.X - size.X) / 2, available.Position.Y), size);

    public static Layout TopRight =>
        (available, size) => new Rect(new Int2(available.Right - size.X, available.Top), size);

    public static Layout MiddleLeft =>
        (available, size) => new Rect(new Int2(available.Left, available.Position.Y + (available.Size.Y - size.Y) / 2), size);

    public static Layout Center =>
        (available, size) => available.Center(size);

    public static Layout MiddleRight =>
        (available, size) => new Rect(new Int2(available.Right - size.X, available.Position.Y + (available.Size.Y - size.Y) / 2), size);

    public static Layout BottomLeft =>
        (available, size) => new Rect(new Int2(available.Left, available.Bottom - size.Y), size);

    public static Layout BottomCenter =>
        (available, size) => new Rect(new Int2(available.Position.X + (available.Size.X - size.X) / 2, available.Bottom - size.Y), size);

    public static Layout BottomRight =>
        (available, size) => new Rect(new Int2(available.Right - size.X, available.Bottom - size.Y), size);

    public static Layout Inset(int amount) =>
        Inset(amount, amount);

    public static Layout Inset(int horizontal, int vertical) =>
        (available, size) => new Rect(available.Position + new Int2(horizontal, vertical), size);

    public static Layout Offset(Int2 offset, Layout inner) =>
        (available, size) =>
        {
            var rect = inner(available, size);
            return new Rect(rect.Position + offset, rect.Size);
        };
}