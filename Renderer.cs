namespace Etch;

public record struct Context(Canvas Canvas, Rect Bounds);
public sealed class Renderer
{
    private readonly Canvas _canvas = new();
    private readonly Stream _output = Console.OpenStandardOutput();

    public void Render(IControl root)
    {
        _canvas.Clear();
        Int2 size = root.Measure((Console.WindowWidth, Console.WindowHeight));
        root.Render(new Context(_canvas, new(Int2.Zero, size)));
        
        _output.Write(_canvas.Span);
        _output.Flush();
    }
}