namespace Etch;

public interface IControl
{
    Int2 Measure(Int2 available);
    void Render(Context context);
}