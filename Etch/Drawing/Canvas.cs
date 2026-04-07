using Keystone;
using System.Buffers;

namespace Etch.Drawing;

public sealed class Canvas
{
    private readonly Stream _output;
    private readonly ArrayBufferWriter<byte> _buffer;

    private readonly Arena<byte> _artifacts;
    private readonly List<Command> _commands;

    public Property<Int2> Size { get; }
    public Anchors Anchors { get; }

    public Canvas(Stream output, Int2 size)
    {
        _artifacts = new Arena<byte>();
        _commands = new List<Command>();
        _buffer = new ArrayBufferWriter<byte>();

        _output = output;
        Size = new Property<Int2>(size);
        Anchors = new Anchors(Size);
    }

    private void EnqueueForDraw(IDrawable drawable) =>
        drawable.Draw(new Context(_artifacts, _commands));

    private void EnqueueForClear(IDrawable drawable)
    {
        int blankCount = drawable.Size.Value.X;
        for (int y = 0; y < drawable.Size.Value.Y; y++)
        {
            Int2 movePos = (drawable.Position.Value.X, drawable.Position.Value.Y + y);
            var moveHandle = _artifacts.Allocate<Int2>(1);
            _artifacts.Write(moveHandle, movePos);
            _commands.Add(new Command(Command.Type.Move, moveHandle));

            var blankHandle = _artifacts.Allocate<int>(1);
            _artifacts.Write(blankHandle, blankCount);
            _commands.Add(new Command(Command.Type.Blank, blankHandle));
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
        if (_commands.Count == 0) return;

        foreach (var command in _commands)
            command.Execute(_buffer, _artifacts);
        _commands.Clear();
        _artifacts.Reset();

        _output.Write(_buffer.WrittenSpan);
        _output.Flush();

        _buffer.Clear();
    }
}