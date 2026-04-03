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

        var benchmark = new Canvas(Console.OpenStandardOutput())
            .Watch(logo, title, progress, iteration).Flush();

        int iterations = 0;
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed.TotalSeconds < 10)
        {
            progress.Current.Value = sw.Elapsed.TotalSeconds;
            iteration.Text.Value = iterations.ToString();
            benchmark.Flush();
            iterations++;
        }
        sw.Stop();

        Console.Clear();

        var score = new Label($"Rendered {iterations} times in {sw.Elapsed.TotalSeconds} sec.");
        var result = new Canvas(Console.OpenStandardOutput()).Watch(score).Flush();

        Console.ReadKey();
    }
}