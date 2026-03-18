using Pith.Geometry;
using Pith.Reactive;

namespace Etch.UI;

public sealed class Label : Control
{
    public readonly Property<string> Text;

    public Label(string initial = "")
    {
        Text = Invalidating(initial);
        Size = new((Text.Value.Length, 1));
        Size.Bind(Text, text => (text.Length, 1));
    }

    public override Property<Int2> Size { get; }
    public override void Render(Surface surface) => surface.Write(Text.Value);
}