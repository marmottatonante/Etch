using Etch.Drawing;
using Keystone;

namespace Etch.UI;

public sealed class Progress : IDrawable
{
    public double Minimum { get; }
    public double Maximum { get; }
    public Property<double> Current { get; }
    public IReadOnlyProperty<int> Percentage { get; }

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }
    public IWatchable Content => Percentage;

    public Progress(double minimum, double maximum)
    {
        Minimum = minimum;
        Maximum = maximum;

        Position = new Property<Int2>(Int2.Zero);
        Size = new Property<Int2>((4, 1));

        Current = new(minimum);
        var percentage = new Property<int>(default);
        percentage.Bind(ComputePercentage, Current);
        Percentage = percentage;
    }

    private int ComputePercentage() =>
        Math.Min(100, Math.Max(0, (int)((Current.Value - Minimum) / (Maximum - Minimum) * 100)));

    public void Draw(Context context)
    {
        Span<char> text = stackalloc char[4];
        Percentage.Value.TryFormat(text, out _, "000");
        text[3] = '%';

        context.Move(Position.Value);
        context.Blit(text);
    }
}