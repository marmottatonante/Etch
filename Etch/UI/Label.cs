using Etch.Drawing;
using Keystone;

namespace Etch.UI;

public sealed class Label : IDrawable
{
    public Property<string> Text { get; }

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }
    public IWatchable Content => Text;

    public Label(string initial)
    {
        Text = new(initial);
        Position = new(Int2.Zero);
        Size = new Property<Int2>(ComputeSize, Text);
    }

    private Int2 ComputeSize() => (Text.Value.Length, 1);
    public void Draw(Context context)
    {
        context.Move(Position.Value);
        context.Blit(Text.Value);
    }
}