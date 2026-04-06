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
        var random = new Random();

        var logo = new Image(Figlet.Split('\n')).Anchor(terminal.Anchors.Center, Alignment.Center);
        var title = new Label("Benchmarking").Anchor(terminal.Anchors.Center, Alignment.Center);
        var progress = new Progress(0, 10).Anchor(terminal.Anchors.Center, Alignment.Center);
        var iteration = new Label("").Anchor(terminal.Anchors.Center, Alignment.Center);

        using (terminal.Watch(logo, title, progress, iteration))
        {
            terminal.Render();

            stopwatch.Start();
            while (stopwatch.Elapsed.TotalSeconds < 10)
            {
                //terminal.Size.Value = (random.Next(80, 120), random.Next(24, 40));
                //terminal.DrawDebugBorder();

                progress.Current.Value = stopwatch.Elapsed.TotalSeconds;
                iteration.Text.Value = iterations.ToString();
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