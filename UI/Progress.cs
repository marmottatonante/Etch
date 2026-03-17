using Etch.Core;

namespace Etch.UI;

public sealed class Progress : Control
{
    public readonly double Minimum;
    public readonly double Maximum;
    public readonly Property<double> Current;
    public readonly ReadOnlyProperty<int> Percentage;

    public Progress(double minimum, double maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
        Current = new(minimum);

        var percentage = Invalidating(0);
        percentage.Bind(Current, value => (int)((value - Minimum) / (Maximum - Minimum) * 100));
        Percentage = percentage.AsReadOnly();
    }

    public override Int2 Measure(Int2 available) => new(4, 1);
    public override void Render(Surface surface) => surface.Write($"{Percentage.Value:000}%");
}
