using System.Diagnostics;

namespace Etch;

public static partial class Shell
{
    public static class Metrics
    {
        public static bool Active { get; set; } = false;

        private static readonly Stopwatch _stopwatch = new();

        public static int Frame { get; private set; } = 0;
        
        public static double DrawTime { get; private set; }
        public static double FlushTime { get; private set; }
        public static double DeltaTime { get; private set; }

        public static double TotalDraw { get; private set; } = 0;
        public static double TotalFlush { get; private set; } = 0;
        public static double TotalDelta { get; private set; } = 0;

        public static double LowestDraw { get; private set; } = double.MaxValue;
        public static double LowestFlush { get; private set; } = double.MaxValue;
        public static double LowestDelta { get; private set; } = double.MaxValue;

        public static double HighestDraw { get; private set; } = double.MinValue;
        public static double HighestFlush { get; private set; } = double.MinValue;
        public static double HighestDelta { get; private set; } = double.MinValue;

        public static double AverageDraw => Frame > 0 ? TotalDraw / Frame : 0;
        public static double AverageFlush => Frame > 0 ? TotalFlush / Frame : 0;
        public static double AverageDelta => Frame > 0 ? TotalDelta / Frame : 0;

        public static void Reset()
        {
            Frame = 0;
            TotalDelta = TotalDraw = TotalFlush = 0;
            LowestDelta = LowestDraw = LowestFlush = double.MaxValue;
            HighestDelta = HighestDraw = HighestFlush = double.MinValue;
        }

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
        internal static void EndFrame()
        {
            if (!Active) return;

            FlushTime = _stopwatch.Elapsed.TotalSeconds;
            DeltaTime = DrawTime + FlushTime;
            _stopwatch.Stop();

            Frame++;
            TotalDelta += DeltaTime; TotalDraw += DrawTime; TotalFlush += FlushTime;
            LowestDelta = Math.Min(LowestDelta, DeltaTime);
            LowestDraw = Math.Min(LowestDraw, DrawTime);
            LowestFlush = Math.Min(LowestFlush, FlushTime);
            HighestDelta = Math.Max(HighestDelta, DeltaTime);
            HighestDraw = Math.Max(HighestDraw, DrawTime);
            HighestFlush = Math.Max(HighestFlush, FlushTime);
        }
    }
}