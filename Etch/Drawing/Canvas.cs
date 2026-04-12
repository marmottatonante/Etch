using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public sealed class Canvas
{
    private readonly Stream _output;
    private readonly ArrayBufferWriter<byte> _buffer;

    private readonly HashSet<Rect> _clearQueue;
    private readonly HashSet<IDrawable> _renderQueue;

    public Context Context => new(_buffer);
    public Property<Int2> Size { get; }
    public Anchors Anchors { get; }

    public Canvas(Stream output, Int2 size)
    {
        _clearQueue = []; _renderQueue = [];
        _buffer = new ArrayBufferWriter<byte>();

        _output = output;
        Size = new Property<Int2>(size);
        Anchors = new Anchors(Size);
    }

    private void EnqueueDraw(IDrawable drawable) =>
        _renderQueue.Add(drawable);

    private void EnqueueClear(IDrawable drawable) =>
        _clearQueue.Add(new Rect(drawable.Position.Value, drawable.Size.Value));

    public Cleanup Watch(IDrawable drawable)
    {
        void onChanging() => EnqueueClear(drawable);
        void onChanged() => EnqueueDraw(drawable);

        EnqueueDraw(drawable);
        drawable.Position.Changing += onChanging;
        drawable.Position.Changed += onChanged;
        drawable.Size.Changing += onChanging;
        drawable.Size.Changed += onChanged;
        drawable.Content.Changed += onChanged;

        return new(() => {
            EnqueueClear(drawable);
            drawable.Position.Changing -= onChanging;
            drawable.Position.Changed -= onChanged;
            drawable.Size.Changing -= onChanging;
            drawable.Size.Changed -= onChanged;
            drawable.Content.Changed -= onChanged;
        });
    }
    public Cleanup Watch(params IDrawable[] drawable) =>
        Cleanup.Merge(drawable.Select(Watch).ToArray());

    public void Render()
    {
        if(_clearQueue.Count == 0 && _renderQueue.Count == 0) return;

        foreach(var rect in _clearQueue)
            Context.Blank(rect);
        _clearQueue.Clear();
        foreach(var drawable in _renderQueue)
            drawable.Draw(Context);
        _renderQueue.Clear();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        _buffer.Clear();
    }
}