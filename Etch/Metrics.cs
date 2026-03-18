using System.Diagnostics;

namespace Etch;

public static partial class Shell
{
    public static class Metrics
    {
        public static bool Active { get; set; } = false;

        private static readonly Stopwatch _stopwatch = new();

        public static double DrawTime { get; private set; }
        public static double FlushTime { get; private set; }
        public static double DeltaTime => DrawTime + FlushTime;

        internal static void StartDraw()
        {
            if (!Active) return;
            _stopwatch.Restart();
        }

        internal static void StartFlush()
        {
            if (!Active) return;
            DrawTime = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Restart();
        }

        internal static void Stop()
        {
            if (!Active) return;
            FlushTime = _stopwatch.Elapsed.TotalSeconds;
            _stopwatch.Stop();
        }
    }
}