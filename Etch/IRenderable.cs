using Keystone.Reactivity;

namespace Etch;

public interface IRenderable
{
    IWatchable Position { get; }
    IWatchable Size { get; }
    IWatchable Content { get; }

    void Render(AnsiBuilder builder);
    void Clear(AnsiBuilder builder);
}