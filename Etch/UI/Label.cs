using Pith.Geometry;
using Pith.Reactive;

namespace Etch.UI;

public sealed class Label : Control
{
    public readonly Property<string> Text;
    public Label(string initial = "") => Text = Invalidating(initial);
    public override Int2 Measure(Int2 available) => new(Text.Value.Length, 1);
    public override void Render(Surface surface) => surface.Write(Text.Value);
}