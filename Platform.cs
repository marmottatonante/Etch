using System.Runtime.InteropServices;

namespace Etch;

public static partial class Shell
{
    public static class Platform
    {
        // Enable ANSI processing on legacy Command Prompt
        public static class ConsoleHost
        {
            [DllImport("kernel32.dll")]
            private static extern bool GetConsoleMode(IntPtr handle, out uint mode);

            [DllImport("kernel32.dll")]
            private static extern bool SetConsoleMode(IntPtr handle, uint mode);

            [DllImport("kernel32.dll")]
            private static extern IntPtr GetStdHandle(int handle);

            private const uint ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;
            private const int STD_OUTPUT_HANDLE = -11;

            public static void EnableAnsi()
            {
                if (!OperatingSystem.IsWindows()) return;
                var handle = GetStdHandle(STD_OUTPUT_HANDLE);
                GetConsoleMode(handle, out uint mode);
                SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
            }
        }
    }
}