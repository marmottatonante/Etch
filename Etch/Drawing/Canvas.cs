using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public partial class Canvas
{
    private readonly Stream _output;
    private readonly ArrayBufferWriter<byte> _buffer;

    private readonly HashSet<ICommand> _drawQueue;
    private readonly HashSet<ICommand> _undrawQueue;

    public Property<Int2> Size { get; }
    public Anchors Anchors { get; }

    public Canvas(Stream output, Int2 size)
    {
        _drawQueue = []; _undrawQueue = [];
        _buffer = new ArrayBufferWriter<byte>();

        _output = output;
        Size = new Property<Int2>(size);
        Anchors = new Anchors(Size);
    }

    private void EnqueueForDraw(IDrawable drawable)
    {
        foreach (var command in drawable.GetCommands())
            _drawQueue.Add(command);
    }
    private void EnqueueForUndraw(IDrawable drawable)
    {
        foreach (var command in drawable.GetCommands())
            _undrawQueue.Add(command);
    }

    public Cleanup Watch(IDrawable drawable)
    {
        void onChanging() => EnqueueForUndraw(drawable);
        void onChanged() => EnqueueForDraw(drawable);

        EnqueueForDraw(drawable);
        drawable.Position.Changing += onChanging;
        drawable.Position.Changed += onChanged;
        drawable.Size.Changing += onChanging;
        drawable.Size.Changed += onChanged;
        drawable.Content.Changed += onChanged;

        return new(() => {
            EnqueueForUndraw(drawable);
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
        if (_undrawQueue.Count == 0 && _drawQueue.Count == 0) return this;

        foreach (var command in _undrawQueue)
            command.Undraw(_buffer);

        foreach (var command in _drawQueue)
            command.Draw(_buffer);

        _undrawQueue.Clear();
        _drawQueue.Clear();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        _buffer.Clear();
        return this;
    }
}