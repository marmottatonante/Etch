using Keystone.Observables;

namespace Etch;

public sealed class Progress : Control
{
    public readonly double Minimum;
    public readonly double Maximum;
    public readonly Property<double> Current;
    public readonly IReadOnlyProperty<int> Percentage;

    public Progress(double minimum, double maximum, double initial = 0)
    {
        Minimum = minimum;
        Maximum = maximum;
        Current = new(initial);

        Property<int> percentage = Invalidating(0);
        percentage.Bind(
            () => (int)((Current.Value - Minimum) / (Maximum - Minimum) * 100), 
            Current);
        Percentage = percentage.AsReadOnly();

        Size.Value = (4, 1);
    }

    public override void Draw(Canvas canvas) =>
        canvas.Move(Position.Value).Write($"{Percentage.Value:000}%");
}