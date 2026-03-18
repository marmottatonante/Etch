using Etch.Core;

namespace Etch.UI;

public sealed class Image : Control
{
    public readonly Property<string[]> Lines;
    public Image(string[] initial) => Lines = Invalidating(initial);
    public override Int2 Measure(Int2 available) => new(Lines.Value[0].Length, Lines.Value.Length);
    public override void Render(Surface surface)
    {
        for (int i = 0; i < Lines.Value.Length; i++)
            surface.Write((0, i), Lines.Value[i]);
    }
}