namespace Etch;

public record struct Context(Canvas Canvas, Rect Bounds);
public sealed class Renderer
{
    private readonly Canvas _canvas = new();

    public void Render(IControl root)
    {
        Int2 size = root.Measure((Console.WindowWidth, Console.WindowHeight));
        root.Render(new Context(_canvas, new(Int2.Zero, size)));
    }
}