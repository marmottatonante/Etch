namespace Etch;

public sealed class Property<T>
{
    public bool Dirty { get; private set; } = true;

    private Func<T>? _source;
    public void Bind(Func<T> source) => _source = source;
    public void Unbind() => _source = null;

    private T _value;
    public T Value { get => Get(); set => Set(value); }

    public Property(T initial) { _value = initial; }
    public Property(Func<T> source) { _source = source; _value = Get(); }

    private T Get()
    {
        if (_source is null) return _value;

        T sourced = _source();
        if (EqualityComparer<T>.Default.Equals(_value, sourced)) return _value;

        _value = sourced;
        Dirty = true;

        return _value;
    }

    private void Set(T value)
    {
        if (_source is not null) throw new InvalidOperationException("Cannot set a bound property.");
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;
        _value = value;
        Dirty = true;
    }

    public bool TryGet(out T value)
    {
        value = Get();
        if (!Dirty) return false;
        Dirty = false;
        return true;
    }
}