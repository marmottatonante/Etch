using Keystone.Observables;
using Keystone.Primitives;

namespace Etch;

public partial class Scene(Stream output, Int2 size)
{
    private readonly Canvas _canvas = new(output, size);
    private readonly List<Control> _controls = [];
    private readonly HashSet<Control> _dirty = [];

    public IReadOnlyProperty<Int2> Size => _canvas.Size;
    public readonly Profiler Metrics = new();

    internal void Invalidate(Control control)
    {
        // Can optimize further by clearing only if size changed.
        // Should also optimize to avoid new string allocation.
        for (int y = 0; y < control.Size.Value.Y; y++)
            _canvas.Move(control.Position.Value + new Int2(0, y))
                   .Write(new string(' ', control.Size.Value.X));

        _dirty.Add(control);
    }

    public Scene Add(Control control)
    {
        control.Scene = this;
        _controls.Add(control);
        _dirty.Add(control);
        return this;
    }

    public void Remove(Control control)
    {
        control.Scene = null;
        _controls.Remove(control);
        _dirty.Remove(control);
    }

    public void Render()
    {
        if (_dirty.Count == 0) return;

        Metrics.StartDraw();

        foreach (var control in _dirty)
            control.Draw(_canvas);
        _dirty.Clear();

        Metrics.StartFlush();

        _canvas.Flush();

        Metrics.Stop();
    }
}
