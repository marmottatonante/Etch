using System.Diagnostics;
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
        var logo = new Image(Figlet.Split('\n'));
        var title = new Label("Benchmarking");
        var progress = new Progress(0, 10);
        var iteration = new Label("");

        var benchmark = Canvas.Terminal.Watch(logo, title, progress, iteration)
            .Enqueue(logo, title, progress, iteration).Render();

        int iterations = 0;
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < 10)
        {
            progress.Current.Value = sw.Elapsed.TotalSeconds;
            iteration.Text.Value = iterations.ToString();
            benchmark.Render();
            iterations++;
        }
        sw.Stop();

        Console.Clear();

        var score = new Label($"Rendered {iterations} times in {sw.Elapsed.TotalSeconds} sec.");
        Canvas.Terminal.Enqueue(score).Render();

        Console.ReadKey();
    }
}