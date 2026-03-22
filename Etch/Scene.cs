namespace Etch;

public class Scene(Stream output)
{
    private readonly Stream _output = output;
    private readonly AnsiBuilder _ansiBuilder = new();

    public void Manage(params IRenderable[] renderables)
    {
        foreach (IRenderable renderable in renderables)
        {
            renderable.Position.Changing += () => renderable.Clear(_ansiBuilder);
            renderable.Position.Changed += () => renderable.Render(_ansiBuilder);
            renderable.Size.Changing += () => renderable.Clear(_ansiBuilder);
            renderable.Content.Changed += () => renderable.Render(_ansiBuilder);
        }
    }

    public void Flush()
    {
        if (_ansiBuilder.Buffer.WrittenCount == 0) return;

        _output.Write(_ansiBuilder.Buffer.WrittenSpan);
        _output.Flush();

        _ansiBuilder.Buffer.Clear();
    }
}