using System.Diagnostics;
using Etch.UI;
using Keystone;

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
        var stopwatch = new Stopwatch();
        int iterations = 0;

        var logo = new Image(Figlet.Split('\n'));
        var title = new Label("Benchmarking");
        var progress = new Progress(0, 10);
        var iteration = new Label("");

        using (Canvas.Terminal.Watch(logo, title, progress, iteration))
        {
            Canvas.Terminal.Render();

            stopwatch.Start();
            while (stopwatch.Elapsed.TotalSeconds < 10)
            {
                progress.Current.Value = stopwatch.Elapsed.TotalSeconds;
                iteration.Text.Value = iterations.ToString();
                Canvas.Terminal.Render();
                iterations++;
            }
            stopwatch.Stop();
        }

        var score = new Label($"Rendered {iterations} times in {stopwatch.Elapsed.TotalSeconds} sec.");
        using (Canvas.Terminal.Watch(score))
            Canvas.Terminal.Render();

        Console.ReadKey();
        Canvas.Terminal.Render();
    }
}