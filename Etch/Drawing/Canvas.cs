using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public partial class Canvas
{
    private readonly Stream _output;
    private readonly ArrayBufferWriter<byte> _buffer;

    private readonly HashSet<Command> _queue;

    public Property<Int2> Size { get; }
    public Anchors Anchors { get; }

    public Canvas(Stream output, Int2 size)
    {
        _queue = [];
        _buffer = new ArrayBufferWriter<byte>();

        _output = output;
        Size = new Property<Int2>(size);
        Anchors = new Anchors(Size);
    }

    private void Draw(IDrawable drawable)
    {
        foreach (var command in drawable.GetCommands())
            _queue.Add(command);
    }

    private void Clear(IDrawable drawable)
    {
        for(int y = 0; y < drawable.Size.Value.Y; y++)
        {
            Int2 position = (drawable.Position.Value.X, drawable.Position.Value.Y + y);
            _queue.Add(Command.Blank(position, drawable.Size.Value.X));
        }
    }

    public Cleanup Watch(IDrawable drawable)
    {
        void onChanging() => Clear(drawable);
        void onChanged() => Draw(drawable);

        Draw(drawable);
        drawable.Position.Changing += onChanging;
        drawable.Position.Changed += onChanged;
        drawable.Size.Changing += onChanging;
        drawable.Size.Changed += onChanged;
        drawable.Content.Changed += onChanged;

        return new(() => {
            Clear(drawable);
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
        if (_queue.Count == 0) return this;

        foreach (var command in _queue)
            command.Execute(_buffer);

        _queue.Clear();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        _buffer.Clear();
        return this;
    }

    public Canvas DrawDebugBorder(Color color = Color.Red)
    {
        var size = Size.Value;
        for (int x = 0; x < size.X; x++)
        {
            _queue.Add(Command.Plot((x, 0), (byte)'-', color, Color.Black));
            _queue.Add(Command.Plot((x, size.Y - 1), (byte)'-', color, Color.Black));
        }
        for (int y = 0; y < size.Y; y++)
        {
            _queue.Add(Command.Plot((0, y), (byte)'|', color, Color.Black));
            _queue.Add(Command.Plot((size.X - 1, y), (byte)'|', color, Color.Black));
        }
        _queue.Add(Command.Plot((0, 0), (byte)'+', color, Color.Black));
        _queue.Add(Command.Plot((size.X - 1, 0), (byte)'+', color, Color.Black));
        _queue.Add(Command.Plot((0, size.Y - 1), (byte)'+', color, Color.Black));
        _queue.Add(Command.Plot((size.X - 1, size.Y - 1), (byte)'+', color, Color.Black));
        return this;
    }
}