using Keystone.Observables;
using Keystone.Primitives;

namespace Etch;

public sealed class Image : Control
{
    public readonly Property<string[]> Lines;

    public Image(string[] initial)
    {
        Lines = Invalidating(initial);
        Size.Bind(
            () => new Int2(Lines.Value.Max(l => l.Length), Lines.Value.Length),
            Lines);
    }

    public override void Draw(Canvas canvas)
    {
        for (int y = 0; y < Lines.Value.Length; y++)
            canvas.Move(Position.Value + new Int2(0, y))
                  .Write(Lines.Value[y]);
    }
}