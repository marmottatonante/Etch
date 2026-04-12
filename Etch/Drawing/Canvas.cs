using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public sealed class Canvas
{
    private readonly Stream _output;
    private readonly ArrayBufferWriter<byte> _buffer;

    private readonly HashSet<Rect> _clearSet;
    private readonly HashSet<IDrawable> _renderSet;

    private readonly List<Rect> _clearList;
    private readonly List<IDrawable> _renderList;

    public Context Context => new(_buffer);
    public Property<Int2> Size { get; }
    public Anchors Anchors { get; }

    public Canvas(Stream output, Int2 size)
    {
        _clearSet = []; _renderSet = [];
        _clearList = []; _renderList = [];
        _buffer = new ArrayBufferWriter<byte>();

        _output = output;
        Size = new Property<Int2>(size);
        Anchors = new Anchors(Size);
    }

    private void EnqueueDraw(IDrawable drawable)
    {
        if(_renderSet.Add(drawable))
            _renderList.Add(drawable);
    }

    private void EnqueueClear(IDrawable drawable)
    {
        Rect rectToClear = new(drawable.Position.Value, drawable.Size.Value);
        if(_clearSet.Add(rectToClear))
            _clearList.Add(rectToClear);
    }

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
        if(_clearList.Count == 0 && _renderList.Count == 0) return;

        foreach(var rect in _clearList)
            Context.Blank(rect);
        _clearSet.Clear();
        _clearList.Clear();
        foreach(var drawable in _renderList)
            drawable.Draw(Context);
        _renderSet.Clear();
        _renderList.Clear();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        _buffer.Clear();
    }
}