using Etch.Drawing;
using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch.UI;

public sealed class Image : IDrawable
{
    public Property<string[]> Lines { get; }

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }
    public IWatchable Content => Lines;

    public Image(string[] lines)
    {
        Lines = new(lines);
        Position = new(Int2.Zero);
        Size = new Property<Int2>(ComputeSize, Lines);
    }

    private Int2 ComputeSize() => Lines.Value.Length == 0 ? Int2.Zero
        : (Lines.Value.Max(l => l.Length), Lines.Value.Length);
    public ICommand[] GetCommands()
    {
        var commands = new ICommand[Lines.Value.Length];
        for (int i = 0; i < Lines.Value.Length; i++)
            commands[i] = new Blit((Position.Value.X, Position.Value.Y + i), Lines.Value[i]);
        return commands;
    }
}