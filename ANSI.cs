using System.Text;

namespace Etch;

public static class ANSI
{
    public static readonly byte[] Reset = "\x1b[0m"u8.ToArray();
    public static readonly byte[] Bold = "\x1b[1m"u8.ToArray();
    public static readonly byte[] Underline = "\x1b[4m"u8.ToArray();
    public static byte[] MoveTo(Int2 position) => 
        Encoding.UTF8.GetBytes($"\x1b[{position.Y + 1};{position.X + 1}H");
    public static byte[] Color(int foreground, int background) => 
        Encoding.UTF8.GetBytes($"\x1b[38;5;{foreground}m\x1b[48;5;{background}m");
}