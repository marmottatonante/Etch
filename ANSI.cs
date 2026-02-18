using System.Text;

namespace Etch;

public static class ANSI
{
    public static readonly byte[][][] MoveTo = CacheMoveTo(120, 80);
    private static byte[][][] CacheMoveTo(int width, int height)
    {
        var cache = new byte[height][][];
        for (int y = 0; y < height; y++)
        {
            cache[y] = new byte[width][];
            for (int x = 0; x < width; x++)
            {
                cache[y][x] = Encoding.UTF8.GetBytes($"\x1b[{y + 1};{x + 1}H");
            }
        }
        return cache;
    }
}