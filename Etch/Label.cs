using Keystone.Observables;

namespace Etch;

public sealed class Label : Control
{
    public readonly Property<string> Text;

    public Label(string initial = "")
    {
        Text = Invalidating(initial);
        Size.Bind(() => (Text.Value.Length, 1), Text);
    }

    public override void Draw(Canvas canvas) =>
        canvas.Move(Position.Value).Write(Text.Value);
}