using System.Runtime.CompilerServices;

namespace Etch;

public class Atlas<T>(int initialSize = 4096) where T : struct
{
    public readonly record struct Handle(int Position, int Length)
    {
        public static Handle Invalid => new(-1, 0);
    }

    private T[] _buffer = new T[initialSize];
    private int _pointer = 0;

    public ReadOnlySpan<T> Data => _buffer.AsSpan(0, _pointer);
    public int Capacity => _buffer.Length;
    public int Used => _pointer;

    public void Reset() => _pointer = 0;
    public void Grow(int minimumCapacity)
    {
        int newCapacity = Math.Max(_buffer.Length * 2, minimumCapacity);
        T[] newBuffer = new T[newCapacity];
        Array.Copy(_buffer, newBuffer, _pointer);
        _buffer = newBuffer;
    }

    public Handle Reserve(int length)
    {
        if(length <= 0) return Handle.Invalid;
        if(_pointer + length > _buffer.Length)
            Grow(_pointer + length);

        int start = _pointer;
        _pointer += length;
        return new(start, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool IsHandleValid(Handle handle)
        => handle.Position >= 0 && handle.Length > 0 
        && handle.Position + handle.Length <= _pointer;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Span<T> Write(Handle handle)
    {
        if (!IsHandleValid(handle)) return Span<T>.Empty;
        return _buffer.AsSpan(handle.Position, handle.Length);
    }
}