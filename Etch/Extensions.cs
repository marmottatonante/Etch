using Keystone.Geometry;

namespace Etch;

public static class Extensions
{
    public enum Alignment { Start, Center, End };

    public static T BottomOf<T>(this T layoutable, ILayoutable target, int spacing = 0, Alignment alignment = Alignment.Start) where T : ILayoutable
    {
        layoutable.Position.Bind(
            () =>
            {
                var x = alignment switch
                {
                    Alignment.Center => target.Position.Value.X + (target.Size.Value.X - layoutable.Size.Value.X) / 2,
                    Alignment.End => target.Position.Value.X + target.Size.Value.X - layoutable.Size.Value.X,
                    _ => target.Position.Value.X
                };
                return new Int2(x, target.Position.Value.Y + target.Size.Value.Y + spacing);
            },
            target.Position, target.Size, layoutable.Size);
        return layoutable;
    }

    public static T RightOf<T>(this T layoutable, ILayoutable target, int spacing = 0, Alignment alignment = Alignment.Start) where T : ILayoutable
    {
        layoutable.Position.Bind(
            () =>
            {
                var y = alignment switch
                {
                    Alignment.Center => target.Position.Value.Y + (target.Size.Value.Y - layoutable.Size.Value.Y) / 2,
                    Alignment.End => target.Position.Value.Y + target.Size.Value.Y - layoutable.Size.Value.Y,
                    _ => target.Position.Value.Y
                };
                return new Int2(target.Position.Value.X + target.Size.Value.X + spacing, y);
            },
            target.Position, target.Size, layoutable.Size);
        return layoutable;
    }
}