using Pith.Geometry;
using Pith.Reactive;

namespace Etch.UI;

public sealed class Progress : Control
{
    public readonly double Minimum;
    public readonly double Maximum;
    public readonly Property<double> Current;
    public readonly IReadOnlyProperty<int> Percentage;

    public Progress(double minimum, double maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
        Current = new(minimum);

        var percentage = Invalidating(0);
        percentage.Bind(Current, value => (int)((value - Minimum) / (Maximum - Minimum) * 100));
        Percentage = percentage.AsReadOnly();
    }

    public Progress(double minimum, double maximum, Property<double> source)
        : this(minimum, maximum) => Current.Bind(source);

    public override Property<Int2> Size { get; } = new((4, 1));
    public override void Render(Surface surface) => surface.Write($"{Percentage.Value:000}%");
}