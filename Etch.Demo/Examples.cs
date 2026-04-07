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
        Canvas terminal = new(Console.OpenStandardOutput(), (Console.WindowWidth, Console.WindowHeight));

        int iterations = 0;
        var stopwatch = new Stopwatch();

        var logo = new Image(Figlet.Split('\n')).Anchor(terminal.Anchors.Center, Alignment.Center, (0, -2));
        var title = new Label("Benchmarking").Anchor(terminal.Anchors.Center, Alignment.Center, (0, 2));
        var progress = new Progress(0, 10).Anchor(terminal.Anchors.Center, Alignment.Center, (0, 3));
        var counter = new Label("").Anchor(terminal.Anchors.Center, Alignment.Center, (0, 3));

        using (terminal.Watch(logo, title, progress, counter))
        {
            terminal.Render();

            stopwatch.Start();
            while (stopwatch.Elapsed.TotalSeconds < 10)
            {
                progress.Current.Value = stopwatch.Elapsed.TotalSeconds;
                counter.Text.Value = iterations.ToString();
                terminal.Render();
                iterations++;
            }
            stopwatch.Stop();
        }

        var score = new Label($"Rendered {iterations} times in {stopwatch.Elapsed.TotalSeconds} sec.");
        using (terminal.Watch(score))
            terminal.Render();
    }
}