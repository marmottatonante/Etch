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
        : this(new Property<Int2>(Int2.Zero), size) { }

    public Anchors(IReadOnlyProperty<Int2> position, IReadOnlyProperty<Int2> size)
    {
        var topLeft = new Property<Int2>(Int2.Zero);
        topLeft.Bind(() => position.Value, position);
        TopLeft = topLeft;

        var topCenter = new Property<Int2>(Int2.Zero);
        topCenter.Bind(() => position.Value + (size.Value.X / 2, 0), position, size);
        TopCenter = topCenter;

        var topRight = new Property<Int2>(Int2.Zero);
        topRight.Bind(() => position.Value + (size.Value.X, 0), position, size);
        TopRight = topRight;

        var middleLeft = new Property<Int2>(Int2.Zero);
        middleLeft.Bind(() => position.Value + (0, size.Value.Y / 2), position, size);
        MiddleLeft = middleLeft;

        var center = new Property<Int2>(Int2.Zero);
        center.Bind(() => position.Value + (size.Value.X / 2, size.Value.Y / 2), position, size);
        Center = center;

        var middleRight = new Property<Int2>(Int2.Zero);
        middleRight.Bind(() => position.Value + (size.Value.X, size.Value.Y / 2), position, size);
        MiddleRight = middleRight;

        var bottomLeft = new Property<Int2>(Int2.Zero);
        bottomLeft.Bind(() => position.Value + (0, size.Value.Y), position, size);
        BottomLeft = bottomLeft;

        var bottomCenter = new Property<Int2>(Int2.Zero);
        bottomCenter.Bind(() => position.Value + (size.Value.X / 2, size.Value.Y), position, size);
        BottomCenter = bottomCenter;

        var bottomRight = new Property<Int2>(Int2.Zero);
        bottomRight.Bind(() => position.Value + size.Value, position, size);
        BottomRight = bottomRight;
    }
}