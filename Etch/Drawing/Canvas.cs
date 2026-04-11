using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public sealed class Canvas
{
    private readonly Stream _output;
    private readonly ArrayBufferWriter<byte> _buffer;

    public Property<Int2> Size { get; }
    public Anchors Anchors { get; }

    public Canvas(Stream output, Int2 size)
    {
        _buffer = new ArrayBufferWriter<byte>();

        _output = output;
        Size = new Property<Int2>(size);
        Anchors = new Anchors(Size);
    }

    private void EnqueueForDraw(IDrawable drawable) =>
        drawable.Draw(new Context(_buffer));

    private void EnqueueForClear(IDrawable drawable)
    {
        var context = new Context(_buffer);
        int blankCount = drawable.Size.Value.X;
        for (int y = 0; y < drawable.Size.Value.Y; y++)
        {
            context.Move((drawable.Position.Value.X, drawable.Position.Value.Y + y));
            context.Blank(blankCount);
        }
    }

    public Cleanup Watch(IDrawable drawable)
    {
        void onChanging() => EnqueueForClear(drawable);
        void onChanged() => EnqueueForDraw(drawable);

        EnqueueForDraw(drawable);
        drawable.Position.Changing += onChanging;
        drawable.Position.Changed += onChanged;
        drawable.Size.Changing += onChanging;
        drawable.Content.Changed += onChanged;

        return new(() => {
            EnqueueForClear(drawable);
            drawable.Position.Changing -= onChanging;
            drawable.Position.Changed -= onChanged;
            drawable.Size.Changing -= onChanging;
            drawable.Content.Changed -= onChanged;
        });
    }
    public Cleanup Watch(params IDrawable[] drawable) =>
        Cleanup.Merge(drawable.Select(Watch).ToArray());

    public void Flush()
    {
        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        _buffer.Clear();
    }
}