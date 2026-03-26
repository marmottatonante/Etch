using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch;

public sealed class Image : IRenderable, ILayoutable
{
    public Property<string[]> Lines { get; }
    public IWatchable Content => Lines;

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }
    IWatchable IRenderable.Position => Position;
    IWatchable IRenderable.Size => Size;

    public Image(string[] lines)
    {
        Lines = new(lines);

        Position = new(Int2.Zero);
        Size = new Property<Int2>(ComputeSize, Lines);
    }

    private Int2 ComputeSize() => Lines.Value.Length == 0 ? Int2.Zero
        : (Lines.Value.Max(l => l.Length), Lines.Value.Length);

    public void Render(Canvas canvas)
    {
        for (int i = 0; i < Size.Value.Y; i++)
            canvas.Move((Position.Value.X, Position.Value.Y + i)).Write(Lines.Value[i]);
    }
    public void Clear(Canvas canvas)
    {
        for (int i = 0; i < Size.Value.Y; i++)
            canvas.Move((Position.Value.X, Position.Value.Y + i)).Blank(Size.Value.X);
    }
}