using Keystone;

namespace Etch.Drawing;

public interface IDrawable : ILayoutable
{
    IWatchable Content { get; }
    void Draw(Context context);
    void Clear(Context context)
    {
        for (int y = 0; y < Size.Value.Y; y++)
        {
            context.Move((Position.Value.X, Position.Value.Y + y));
            context.Blank(Size.Value.X);
        }
    }
}