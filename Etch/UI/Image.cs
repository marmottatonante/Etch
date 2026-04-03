using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch.UI;

public sealed class Image : Widget
{
    public Property<string[]> Lines { get; }

    public override Property<Int2> Position { get; }
    public override IReadOnlyProperty<Int2> Size { get; }
    public override IWatchable Content => Lines;

    public Image(string[] lines)
    {
        Lines = new(lines);

        Position = new(Int2.Zero);
        Size = new Property<Int2>(ComputeSize, Lines);
    }

    private Int2 ComputeSize() => Lines.Value.Length == 0 ? Int2.Zero
        : (Lines.Value.Max(l => l.Length), Lines.Value.Length);

    public override void Render(Canvas canvas)
    {
        for (int i = 0; i < Size.Value.Y; i++)
            canvas.Move((Position.Value.X, Position.Value.Y + i)).Write(Lines.Value[i]);
    }
}