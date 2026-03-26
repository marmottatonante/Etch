using Keystone.Reactivity;

namespace Etch;

public interface IRenderable
{
    IWatchable Position { get; }
    IWatchable Size { get; }
    IWatchable Content { get; }

    void Render(Canvas canvas);
    void Clear(Canvas canvas);
}