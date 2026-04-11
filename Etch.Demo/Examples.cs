using System.Diagnostics;
using Etch.Drawing;
using Etch.UI;

namespace Etch.Demo;

public static class Examples
{
    public const string Figlet =
    """
    '||''''|    .           '||     
     ||  .    .||.    ....   || ..  
     ||''|     ||   .|   ''  ||' || 
     ||        ||   ||       ||  || 
    .||.....|  '|.'  '|...' .||. ||.
    """;

    public static void Benchmark()
    {
        int width = Console.WindowWidth;
        int height = Console.WindowHeight;

        Canvas terminal = new(Console.OpenStandardOutput(), (width, height));

        var rng = new Random(42);
        var stopwatch = new Stopwatch();
        var iterations = 0;

        // 200 labels at random positions updating every frame
        var labels = Enumerable.Range(0, 200).Select(i =>
        {
            var label = new Label(i.ToString());
            label.Position.Value = (rng.Next(0, width - 10), rng.Next(0, height));
            return label;
        }).ToArray();

        using (terminal.Watch(labels))
        {
            terminal.Flush();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalSeconds < 10)
            {
                // update all labels
                foreach (var (label, i) in labels.Select((l, i) => (l, i)))
                    label.Text.Value = $"{i}:{iterations}";

                // move labels around
                foreach (var label in labels)
                    label.Position.Value = (rng.Next(0, width - 10), rng.Next(0, height));

                terminal.Flush();
                iterations++;
            }

            stopwatch.Stop();
        }

        var score = new Label($"Stress test: {iterations} frames in {stopwatch.Elapsed.TotalSeconds:F2}s ({iterations / stopwatch.Elapsed.TotalSeconds:F0} fps)");
        using (terminal.Watch(score))
            terminal.Flush();
    }
}