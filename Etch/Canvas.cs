using Etch.Drawing;
using Keystone.Geometry;
using Keystone.Reactivity;
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

    public Canvas Enqueue(IDrawable drawable)
    {
        EnqueueForDraw(drawable);
        return this;
    }
    public Canvas Enqueue(params IDrawable[] drawables)
    {
        foreach (var drawable in drawables) EnqueueForDraw(drawable);
        return this;
    }
    public Canvas Watch(IDrawable drawable)
    {
        drawable.Position.Changing += () => EnqueueForUndraw(drawable);
        drawable.Position.Changed += () => EnqueueForDraw(drawable);
        drawable.Size.Changing += () => EnqueueForUndraw(drawable);
        drawable.Size.Changed += () => EnqueueForDraw(drawable);
        drawable.Content.Changed += () => EnqueueForDraw(drawable);
        return this;
    }
    public Canvas Watch(params IDrawable[] drawables)
    {
        foreach (var drawable in drawables) Watch(drawable);
        return this;
    }

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