using System.Buffers;
using System.Text;

namespace Etch;

public readonly ref struct Region(ArrayBufferWriter<byte> buffer, Int2 offset)
{
    private readonly ArrayBufferWriter<byte> _buffer = buffer;
    private readonly Int2 _offset = offset;

    public Region Move(Int2 position)
    {
        _buffer.Write(ANSI.MoveTo(position + _offset));
        return this;
    }
    public Region Write(ReadOnlySpan<char> text) 
    {
        int max = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = _buffer.GetSpan(max);
        _buffer.Advance(Encoding.UTF8.GetBytes(text, span));
        return this;
    }
}