using System.Diagnostics;

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
        Scene benchmark = new(Console.OpenStandardOutput());

        var title = new Label("Benchmarking");
        var progress = new Progress(0, 10).BottomOf(title, 0, Layouts.Alignment.Center);
        var iteration = new Label("Waiting").BottomOf(progress, 2, Layouts.Alignment.Center);
        benchmark.Manage(title, progress, iteration);
        benchmark.Flush();

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
        Scene results = new(Console.OpenStandardOutput());

        var score = new Label("");
        results.Manage(score);

        score.Text.Value = $"Rendered {iterations} times in {sw.Elapsed.TotalSeconds} sec.";
        results.Flush();

        Console.ReadKey();
    }
}