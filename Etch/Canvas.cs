using Etch.Drawing;
using Keystone;
using System.Buffers;

namespace Etch;

public sealed class Canvas(Stream output, Int2 size)
{
    private readonly Stream _output = output;

    private readonly HashSet<ICommand> _drawQueue = [];
    private readonly HashSet<ICommand> _undrawQueue = [];

    public ArrayBufferWriter<byte> Buffer { get; } = new();
    public Property<Int2> Size { get; } = new(size);

    public static Canvas Terminal { get; } = 
        new(Console.OpenStandardOutput(), 
            (Console.WindowWidth, Console.WindowHeight));

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
            command.Undraw(Buffer);

        foreach (var command in _drawQueue)
            command.Draw(Buffer);

        _undrawQueue.Clear();
        _drawQueue.Clear();

        _output.Write(Buffer.WrittenSpan);
        _output.Flush();

        Buffer.Clear();
        return this;
    }
}