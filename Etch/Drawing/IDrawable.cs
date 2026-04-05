using Keystone;

namespace Etch.Drawing;

public interface IDrawable : ILayoutable
{
    IWatchable Content { get; }
    ICommand[] GetCommands();
}