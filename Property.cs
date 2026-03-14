namespace Etch;

public class ReadOnlyProperty<T>(T initial)
{
    protected T _value = initial;
    public bool Dirty { get; protected set; } = true;
    public T Peek() => _value;

    public event Action<T>? Changed;
    protected void Invoke(T value) => Changed?.Invoke(value);
}

public sealed class Property<T>(T initial) : ReadOnlyProperty<T>(initial)
{
    public T Get() { Dirty = false; return _value; }
    public void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;
        _value = value;
        Dirty = true;
        Invoke(value);
    }

    public void Bind(Property<T> target) => Changed += target.Set;
    public void Unbind(Property<T> target) => Changed -= target.Set;
}