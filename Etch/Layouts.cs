using Keystone.Primitives;

namespace Etch;

public static class Layouts
{
    public enum Alignment { Start, Center, End };
    public static T BottomOf<T>(this T control, Control target, int spacing = 0, Alignment alignment = Alignment.Start) where T : Control
    {
        control.Position.Bind(
            () =>
            {
                var x = alignment switch
                {
                    Alignment.Center => target.Position.Value.X + (target.Size.Value.X - control.Size.Value.X) / 2,
                    Alignment.End => target.Position.Value.X + target.Size.Value.X - control.Size.Value.X,
                    _ => target.Position.Value.X
                };
                return new Int2(x, target.Position.Value.Y + target.Size.Value.Y + spacing);
            },
            target.Position, target.Size, control.Size);
        return control;
    }

    public static T RightOf<T>(this T control, Control target, int spacing = 0, Alignment alignment = Alignment.Start) where T : Control
    {
        control.Position.Bind(
            () =>
            {
                var y = alignment switch
                {
                    Alignment.Center => target.Position.Value.Y + (target.Size.Value.Y - control.Size.Value.Y) / 2,
                    Alignment.End => target.Position.Value.Y + target.Size.Value.Y - control.Size.Value.Y,
                    _ => target.Position.Value.Y
                };
                return new Int2(target.Position.Value.X + target.Size.Value.X + spacing, y);
            },
            target.Position, target.Size, control.Size);
        return control;
    }
}