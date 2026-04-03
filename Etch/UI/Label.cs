using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch.UI;

public sealed class Label : Widget
{
    public Property<string> Text { get; }

    public override Property<Int2> Position { get; }
    public override IReadOnlyProperty<Int2> Size { get; }
    public override IWatchable Content => Text; 

    public Label(string initial)
    {
        Text = new(initial);

        Position = new(Int2.Zero);
        Size = new Property<Int2>(ComputeSize, Text);
    }

    private Int2 ComputeSize() => (Text.Value.Length, 1);

    public override void Render(Canvas canvas) => 
        canvas.Move(Position.Value).Write(Text.Value);
}