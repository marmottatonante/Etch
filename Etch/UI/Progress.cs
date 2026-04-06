using Etch.Drawing;
using Keystone;

namespace Etch.UI;

public sealed class Progress : IDrawable
{
    public double Minimum { get; }
    public double Maximum { get; }
    public Property<double> Current { get; }
    public Property<int> Percentage { get; }

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }
    public IWatchable Content => Percentage;

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
        Math.Min(100, Math.Max(0, (int)((Current.Value - Minimum) / (Maximum - Minimum) * 100)));
    public Command[] GetCommands() => [Command.Blit(Position.Value, $"{Percentage.Value:000}%")];
}