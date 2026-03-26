using Keystone.Geometry;
using Keystone.Reactivity;

namespace Etch;

public sealed class Progress : IRenderable, ILayoutable
{
    public double Minimum { get; }
    public double Maximum { get; }
    public Property<double> Current { get; }
    public Property<int> Percentage { get; }
    public IWatchable Content => Percentage;

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }
    IWatchable IRenderable.Position => Position;
    IWatchable IRenderable.Size => Size;

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

    public void Render(Canvas canvas) =>
        canvas.Move(Position.Value).Write($"{Percentage.Value:000}%");
    public void Clear(Canvas canvas) =>
        canvas.Move(Position.Value).Blank(Size.Value.X);
}