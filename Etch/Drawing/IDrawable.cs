using Keystone.Reactivity;

namespace Etch.Drawing;

public interface IDrawable : ILayoutable
{
    IWatchable Content { get; }
    ICommand[] GetCommands();
}