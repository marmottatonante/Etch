using Keystone.Geometry;
using System.Buffers;
using System.Text;

namespace Etch;

public sealed class Canvas(Stream output)
{
    private readonly Stream _output = output;

    public ArrayBufferWriter<byte> Buffer { get; } = new();

    public Canvas Clear() { Buffer.Write("\x1b[2J"u8); return this; }
    public Canvas Reset() { Buffer.Write("\x1b[0m"u8); return this; }

    private Canvas Color(ReadOnlySpan<byte> prefix, Color color)
    {
        byte bColor = (byte)color;
        var span = Buffer.GetSpan(11);
        int written = 0;
        prefix.CopyTo(span[written..]); written += prefix.Length;
        bColor.TryFormat(span[written..], out int fw, default, null); written += fw;
        span[written++] = (byte)'m';
        Buffer.Advance(written);
        return this;
    }

    public Canvas Foreground(Color color) => Color("\x1b[38;5;"u8, color);
    public Canvas Background(Color color) => Color("\x1b[48;5;"u8, color);

    public Canvas Move(Int2 position)
    {
        var span = Buffer.GetSpan(14);
        int written = 0;
        "\x1b["u8.CopyTo(span[written..]); written += 2;
        (position.Y + 1).TryFormat(span[written..], out int rw, default, null); written += rw;
        span[written++] = (byte)';';
        (position.X + 1).TryFormat(span[written..], out int cw, default, null); written += cw;
        span[written++] = (byte)'H';
        Buffer.Advance(written);
        return this;
    }

    public Canvas Write(byte b) { Buffer.GetSpan(1)[0] = b; Buffer.Advance(1); return this; }
    public Canvas Write(ReadOnlySpan<char> text)
    {
        int bytes = Encoding.UTF8.GetMaxByteCount(text.Length);
        var span = Buffer.GetSpan(bytes);
        Buffer.Advance(Encoding.UTF8.GetBytes(text, span));
        return this;
    }

    public Canvas Blank(int count)
    {
        var span = Buffer.GetSpan(count);
        span[..count].Fill((byte)' ');
        Buffer.Advance(count);
        return this;
    }

    public Canvas Watch(IRenderable renderable)
    {
        renderable.Position.Changing += () => renderable.Clear(this);
        renderable.Position.Changed += () => renderable.Render(this);
        renderable.Size.Changing += () => renderable.Clear(this);
        renderable.Content.Changed += () => renderable.Render(this);

        renderable.Render(this);
        return this;
    }
    public Canvas Watch(params IRenderable[] renderables)
    {
        foreach(IRenderable renderable in renderables)
            Watch(renderable);

        return this;
    }

    public Canvas Flush()
    {
        if (Buffer.WrittenCount == 0) return this;

        _output.Write(Buffer.WrittenSpan);
        _output.Flush();

        Buffer.Clear();
        return this;
    }
}