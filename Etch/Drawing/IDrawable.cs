using Keystone;

namespace Etch.Drawing;

public interface IDrawable : ILayoutable
{
    IWatchable Content { get; }
    void Draw(Context context);
}