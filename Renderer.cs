namespace Etch;

public record struct Context(Canvas Canvas, Rect Bounds);
public sealed class Renderer
{
    private readonly Canvas _canvas = new();

    public void Render(IControl root)
    {
        root.Render(new Context(_canvas, new()));
    }
}