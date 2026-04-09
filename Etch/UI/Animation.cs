using Etch.Drawing;
using Keystone;

namespace Etch.UI;

public sealed class Animation : IDrawable
{
    private readonly string[][] _frames;

    public Property<Int2> Position { get; }
    public IReadOnlyProperty<Int2> Size { get; }
    public Property<int> Frame { get; }
    public IWatchable Content => Frame;

    public Animation(string[][] frames)
    {
        _frames = frames;
        Frame = new(0);
        Position = new Property<Int2>(Int2.Zero);
        Size = new Property<Int2>(ComputeSize());
    }

    public Animation(string[] frames)
        : this(frames.Select(f => new[] { f }).ToArray()) { }

    private Int2 ComputeSize()
    {
        int width = _frames.Max(frame => frame.Max(l => l.Length));
        int height = _frames.Max(frame => frame.Length);
        return (width, height);
    }

    public void Advance() => Frame.Value = (Frame.Value + 1) % _frames.Length;

    public void Draw(Context context)
    {
        var frame = _frames[Frame.Value];
        for (int i = 0; i < frame.Length; i++)
        {
            context.Move((Position.Value.X, Position.Value.Y + i));
            context.Blit(frame[i]);
        }
    }
}