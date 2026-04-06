using Keystone;
using System.Buffers;
using System.Text;

namespace Etch.Drawing;

public sealed class Canvas
{
    private readonly Stream _output;
    private readonly ArrayBufferWriter<byte> _buffer;

    private readonly HashSet<IDrawable> _drawQueue;
    private readonly HashSet<IDrawable> _clearQueue;

    public Property<Int2> Size { get; }
    public Anchors Anchors { get; }

    public Canvas(Stream output, Int2 size)
    {
        _drawQueue = []; _clearQueue = [];
        _buffer = new ArrayBufferWriter<byte>();

        _output = output;
        Size = new Property<Int2>(size);
        Anchors = new Anchors(Size);
    }

    public Cleanup Watch(IDrawable drawable)
    {
        void onChanging() => _clearQueue.Add(drawable);
        void onChanged() => _drawQueue.Add(drawable);

        _drawQueue.Add(drawable);
        drawable.Position.Changing += onChanging;
        drawable.Position.Changed += onChanged;
        drawable.Size.Changing += onChanging;
        drawable.Size.Changed += onChanged;
        drawable.Content.Changed += onChanged;

        return new(() => {
            _clearQueue.Add(drawable);
            drawable.Position.Changing -= onChanging;
            drawable.Position.Changed -= onChanged;
            drawable.Size.Changing -= onChanging;
            drawable.Size.Changed -= onChanged;
            drawable.Content.Changed -= onChanged;
        });
    }
    public Cleanup Watch(params IDrawable[] drawable) =>
        Cleanup.Merge(drawable.Select(Watch).ToArray());

    public Canvas Render()
    {
        if (_drawQueue.Count == 0 && _clearQueue.Count == 0) return this;

        foreach(var drawable in _clearQueue)
            drawable.Clear(new Context(_buffer));
        _clearQueue.Clear();

        foreach (var drawable in _drawQueue)
            drawable.Draw(new Context(_buffer));
        _drawQueue.Clear();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        _buffer.Clear();
        return this;
    }
}