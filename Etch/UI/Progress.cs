using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch.UI;

public sealed class Progress : Widget
{
    public double Minimum { get; }
    public double Maximum { get; }
    public Property<double> Current { get; }
    public Property<int> Percentage { get; }

    public override Property<Int2> Position { get; }
    public override IReadOnlyProperty<Int2> Size { get; }
    public override IWatchable Content => Percentage;

    public Progress(double minimum, double maximum)
    {
        Minimum = minimum;
        Maximum = maximum;
        Current = new(minimum);
        Percentage = new(ComputePercentage, Current);

        Position = new(Int2.Zero);
        Size = new Property<Int2>((4, 1));
    }

    private int ComputePercentage() => 
        (int)((Current.Value - Minimum) / (Maximum - Minimum) * 100);

    public override void Render(Canvas canvas) =>
        canvas.Move(Position.Value).Write($"{Percentage.Value:000}%");
}