using Keystone;

namespace Etch.Drawing;

public enum Alignment
{
    TopLeft,
    TopCenter,
    TopRight,
    MiddleLeft,
    Center,
    MiddleRight,
    BottomLeft,
    BottomCenter,
    BottomRight,
}

public static class Layouts
{
    public static T Anchor<T>(this T layoutable, IReadOnlyProperty<Int2> anchor, Alignment alignment = Alignment.TopLeft, Int2 margin = default)
        where T : ILayoutable
    {
        layoutable.Position.Bind(() =>
        {
            Int2 size = layoutable.Size.Value;
            Int2 offset = alignment switch
            {
                Alignment.TopLeft => Int2.Zero,
                Alignment.TopCenter => (-size.X / 2, 0),
                Alignment.TopRight => (-size.X, 0),
                Alignment.MiddleLeft => (0, -size.Y / 2),
                Alignment.Center => (-size.X / 2, -size.Y / 2),
                Alignment.MiddleRight => (-size.X, -size.Y / 2),
                Alignment.BottomLeft => (0, -size.Y),
                Alignment.BottomCenter => (-size.X / 2, -size.Y),
                Alignment.BottomRight => (-size.X, -size.Y),
                _ => Int2.Zero
            };
            return anchor.Value + offset + margin;
        }, anchor, layoutable.Size);
        return layoutable;
    }
}
