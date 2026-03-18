using Pith.Geometry;
using Pith.Reactive;

namespace Etch.UI;

public sealed class Image : Control
{
    public readonly Property<string[]> Lines;

    public Image(string[] initial)
    {
        Lines = Invalidating(initial);
        Size = new((Lines.Value.Max(l => l.Length), Lines.Value.Length));
        Size.Bind(Lines, lines => (lines.Max(l => l.Length), lines.Length));
    }

    public override Property<Int2> Size { get; }
    public override void Render(Surface surface)
    {
        for (int i = 0; i < Lines.Value.Length; i++)
            surface.Write((0, i), Lines.Value[i]);
    }
}