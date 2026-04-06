using Keystone;

namespace Etch.Drawing;

public sealed class Anchors
{
    public IReadOnlyProperty<Int2> TopLeft { get; }
    public IReadOnlyProperty<Int2> TopCenter { get; }
    public IReadOnlyProperty<Int2> TopRight { get; }
    public IReadOnlyProperty<Int2> MiddleLeft { get; }
    public IReadOnlyProperty<Int2> Center { get; }
    public IReadOnlyProperty<Int2> MiddleRight { get; }
    public IReadOnlyProperty<Int2> BottomLeft { get; }
    public IReadOnlyProperty<Int2> BottomCenter { get; }
    public IReadOnlyProperty<Int2> BottomRight { get; }

    public Anchors(IReadOnlyProperty<Int2> size)
    {
        TopLeft = new Property<Int2>(() => Int2.Zero, size);
        TopCenter = new Property<Int2>(() => (size.Value.X / 2, 0), size);
        TopRight = new Property<Int2>(() => (size.Value.X, 0), size);
        MiddleLeft = new Property<Int2>(() => (0, size.Value.Y / 2), size);
        Center = new Property<Int2>(() => (size.Value.X / 2, size.Value.Y / 2), size);
        MiddleRight = new Property<Int2>(() => (size.Value.X, size.Value.Y / 2), size);
        BottomLeft = new Property<Int2>(() => (0, size.Value.Y), size);
        BottomCenter = new Property<Int2>(() => (size.Value.X / 2, size.Value.Y), size);
        BottomRight = new Property<Int2>(() => size.Value, size);
    }

    public Anchors(IReadOnlyProperty<Int2> position, IReadOnlyProperty<Int2> size)
    {
        TopLeft = new Property<Int2>(() => position.Value, position, size);
        TopCenter = new Property<Int2>(() => position.Value + (size.Value.X / 2, 0), position, size);
        TopRight = new Property<Int2>(() => position.Value + (size.Value.X, 0), position, size);
        MiddleLeft = new Property<Int2>(() => position.Value + (0, size.Value.Y / 2), position, size);
        Center = new Property<Int2>(() => position.Value + (size.Value.X / 2, size.Value.Y / 2), position, size);
        MiddleRight = new Property<Int2>(() => position.Value + (size.Value.X, size.Value.Y / 2), position, size);
        BottomLeft = new Property<Int2>(() => position.Value + (0, size.Value.Y), position, size);
        BottomCenter = new Property<Int2>(() => position.Value + (size.Value.X / 2, size.Value.Y), position, size);
        BottomRight = new Property<Int2>(() => position.Value + size.Value, position, size);
    }
}