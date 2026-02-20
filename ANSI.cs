using System.Text;

namespace Etch;

public static class ANSI
{
    public static readonly byte[] Reset = "\x1b[0m"u8.ToArray();
    public static readonly byte[] Bold = "\x1b[1m"u8.ToArray();
    public static readonly byte[] Underline = "\x1b[4m"u8.ToArray();
    public static byte[] MoveTo(int x, int y) => Encoding.UTF8.GetBytes($"\x1b[{y + 1};{x + 1}H");
}