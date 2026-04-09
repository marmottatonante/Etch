using Etch.Drawing;
using Keystone;

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
        Position = new Property<Int2>(Int2.Zero);

        var size = new Property<Int2>(Int2.Zero);
        size.Bind(ComputeSize, Lines);
        Size = size;
    }

    private Int2 ComputeSize() => Lines.Value.Length == 0 ? Int2.Zero
        : (Lines.Value.Max(l => l.Length), Lines.Value.Length);

    public void Draw(Context context)
    {
        for (int i = 0; i < Lines.Value.Length; i++)
        {
            context.Move((Position.Value.X, Position.Value.Y + i));
            context.Blit(Lines.Value[i]);
        }
    }
}