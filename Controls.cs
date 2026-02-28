using System.Drawing;

namespace Etch;

public interface IControl
{
    Int2 Measure(Int2 available);
    void Draw(Region region);
}

public sealed class Label : IControl
{
    public readonly Property<string> Text;
    public readonly Property<Int2> Size;

    public Label(string initial)
    {
        Text = new(initial);
        Size = new(() => new(Text.Value.Length, 1));
    }

    public Int2 Measure(Int2 available)
    {
        Size.TryGet(out _);
        return Size.Value;
    }
    public void Draw(Region region)
    {
        if (!Text.TryGet(out var text)) return;
        region.Write(text);
    }
}

public sealed class Image : IControl
{
    public readonly Property<string[]> Lines;
    public readonly Property<Int2> Size;

    public Image(string[] lines)
    {
        Lines = new(lines);
        Size = new(() => (Lines.Value.Max(l => l.Length), Lines.Value.Length));
    }

    public Int2 Measure(Int2 available)
    {
        Size.TryGet(out _);
        return Size.Value;
    }
    public void Draw(Region region)
    {
        if (!Lines.TryGet(out var lines)) return;
        for (int i = 0; i < lines.Length; i++)
            region.Write((0, i), lines[i].AsSpan());
    }
}

public sealed class Center(IControl child) : IControl
{
    public readonly IControl Child = child;
    public Int2 Measure(Int2 available) => available;
    public void Draw(Region region)
    {
        if (!Child.Size.TryGet(out var childSize)) return;
        Child.Draw(region.Slice(region.Bounds.Center(childSize)));
    }
}