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

        var labels = Enumerable.Range(0, 200).Select(i =>
        {
            var label = new Label(i.ToString());
            label.Position.Value = (rng.Next(0, width - 10), rng.Next(0, height));
            return label;
        }).ToArray();

        using (terminal.Watch(labels))
        {
            terminal.Render();
            stopwatch.Start();

            while (stopwatch.Elapsed.TotalSeconds < 10)
            {
                foreach (var (label, i) in labels.Select((l, i) => (l, i)))
                    label.Text.Value = $"{i}:{iterations}";

                foreach (var label in labels)
                    label.Position.Value = (rng.Next(0, width - 10), rng.Next(0, height));

                terminal.Render();
                iterations++;
            }

            stopwatch.Stop();
        }

        var logo = new Image(Figlet.Split('\n')).Anchor(terminal.Anchors.Center, Alignment.Center, (0, -2));
        var score = new Label($"{iterations} in {stopwatch.Elapsed.TotalSeconds:F2}s")
            .Anchor(terminal.Anchors.Center, Alignment.Center, (0, 2));
        using (terminal.Watch(logo, score))
            terminal.Render();
        Console.ReadKey();
    }
}