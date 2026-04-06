using Keystone;
using System.Buffers;
using System.Text;

namespace Etch.Drawing;

public sealed class Canvas
{
    private readonly Stream _output;
    private readonly ArrayBufferWriter<byte> _buffer;

    private readonly HashSet<IDrawable> _drawSet;
    private readonly HashSet<IDrawable> _clearSet;
    private readonly List<IDrawable> _drawQueue;
    private readonly List<IDrawable> _clearQueue;

    public Property<Int2> Size { get; }
    public Anchors Anchors { get; }

    public Context Context => new(_buffer);

    public Canvas(Stream output, Int2 size)
    {
        _drawSet = []; _clearSet = [];
        _drawQueue = []; _clearQueue = [];
        _buffer = new ArrayBufferWriter<byte>();

        _output = output;
        Size = new Property<Int2>(size);
        Anchors = new Anchors(Size);
    }

    private void EnqueueForDraw(IDrawable drawable)
    {
        if (_drawSet.Add(drawable))
            _drawQueue.Add(drawable);
    }

    private void EnqueueForClear(IDrawable drawable)
    {
        if (_clearSet.Add(drawable))
            _clearQueue.Add(drawable);
    }

    public Cleanup Watch(IDrawable drawable)
    {
        void onChanging() => EnqueueForClear(drawable);
        void onChanged() => EnqueueForDraw(drawable);

        EnqueueForDraw(drawable);
        drawable.Position.Changing += onChanging;
        drawable.Position.Changed += onChanged;
        drawable.Size.Changing += onChanging;
        drawable.Size.Changed += onChanged;
        drawable.Content.Changed += onChanged;

        return new(() => {
            EnqueueForClear(drawable);
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
        if (_drawQueue.Count == 0 && _clearQueue.Count == 0) return;

        foreach(var drawable in _clearQueue)
            drawable.Clear(Context);
        _clearQueue.Clear();
        _clearSet.Clear();

        foreach (var drawable in _drawQueue)
            drawable.Draw(Context);
        _drawQueue.Clear();
        _drawSet.Clear();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        _buffer.Clear();
    }
}