namespace Etch;

public interface IMutable
{
    event Action? Changed;
}

public sealed class Property<T>(T initial) : IMutable
{
    public event Action? Changed;

    private T _value = initial;
    public T Value
    {
        get => _value;
        set => Set(value);
    }

    private void Set(T value)
    {
        if (EqualityComparer<T>.Default.Equals(_value, value)) return;

        _value = value;
        Changed?.Invoke();
    }
}