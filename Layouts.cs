namespace Etch;
public delegate Rect Layout(Rect screen, Int2 size);
public static class Layouts
{
    // Base layouts
    public static Layout Fill => (screen, _) => screen;
    public static Layout Center => (screen, size) => screen.Center(size);
    public static Layout TopLeft => (screen, size) => new Rect(Int2.Zero, size);
    public static Layout TopRight => (screen, size) => new Rect(new Int2(screen.Size.X - size.X, 0), size);
    public static Layout BottomLeft => (screen, size) => new Rect(new Int2(0, screen.Size.Y - size.Y), size);
    public static Layout BottomRight => (screen, size) => new Rect(screen.Size - size, size);
    public static Layout At(Int2 position) => (_, size) => new Rect(position, size);

    // Modifiers
    public static Layout Offset(this Layout layout, Int2 offset) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(r.Position + offset, r.Size); };

    public static Layout Above(this Layout layout, int gap = 0) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(new Int2(r.Position.X, r.Top - size.Y - gap), size); };

    public static Layout Below(this Layout layout, int gap = 0) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(new Int2(r.Position.X, r.Bottom + gap), size); };

    public static Layout LeftOf(this Layout layout, int gap = 0) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(new Int2(r.Left - size.X - gap, r.Position.Y), size); };

    public static Layout RightOf(this Layout layout, int gap = 0) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(new Int2(r.Right + gap, r.Position.Y), size); };

    public static Layout Deflate(this Layout layout, int amount) =>
        (screen, size) => { var r = layout(screen, size); return r.Deflate(amount); };

    public static Layout Deflate(this Layout layout, int horizontal, int vertical) =>
        (screen, size) => { var r = layout(screen, size); return r.Deflate(horizontal, vertical); };

    public static Layout AlignLeft(this Layout layout) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(new Int2(0, r.Position.Y), r.Size); };

    public static Layout AlignRight(this Layout layout) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(new Int2(screen.Size.X - r.Size.X, r.Position.Y), r.Size); };

    public static Layout AlignTop(this Layout layout) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(new Int2(r.Position.X, 0), r.Size); };

    public static Layout AlignBottom(this Layout layout) =>
        (screen, size) => { var r = layout(screen, size); return new Rect(new Int2(r.Position.X, screen.Size.Y - r.Size.Y), r.Size); };
}