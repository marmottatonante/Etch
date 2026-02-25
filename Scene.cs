using System.Buffers;

namespace Etch;

public sealed class Scene(params (IControl Control, Layout Layout)[] entries)
{
    private readonly (IControl Control, Layout Layout)[] _entries = entries;
    private readonly Rect[] _cache = new Rect[entries.Length];

    internal void Arrange(Rect screen)
    {
        for (int i = 0; i < _entries.Length; i++)
            _cache[i] = _entries[i].Layout(screen, _entries[i].Control.Size);
    }

    internal void Draw(ArrayBufferWriter<byte> buffer)
    {
        for (int i = 0; i < _entries.Length; i++)
            _entries[i].Control.Draw(new Region(buffer, _cache[i]));
    }

    internal void Update(ArrayBufferWriter<byte> buffer)
    {
        for (int i = 0; i < _entries.Length; i++)
            _entries[i].Control.Update(new Region(buffer, _cache[i]));
    }
}