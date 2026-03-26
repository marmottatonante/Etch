using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch;

public sealed class Label : IRenderable, ILayoutable
{
    public Property<string> Text { get; }
    public IWatchable Content => Text; 

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }
    IWatchable IRenderable.Position => Position;
    IWatchable IRenderable.Size => Size;

    public Label(string initial)
    {
        Text = new(initial);

        Position = new(Int2.Zero);
        Size = new Property<Int2>(ComputeSize, Text);
    }

    private Int2 ComputeSize() => (Text.Value.Length, 1);

    public void Render(Canvas canvas) => 
        canvas.Move(Position.Value).Write(Text.Value);
    public void Clear(Canvas canvas) =>
        canvas.Move(Position.Value).Blank(Size.Value.X);
}