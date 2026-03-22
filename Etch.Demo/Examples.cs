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

    public static void HelloWorld()
    {
        Scene helloWorld = new(Console.OpenStandardOutput());

        var title = new Label("Hello");
        var subtitle = new Label("World").BottomOf(title);
        helloWorld.Manage(title, subtitle);

        helloWorld.Flush();
        Console.ReadKey();

        title.Position.Value += (0, 1);
        helloWorld.Flush();

        Console.ReadKey();
    }

    public static void Benchmark()
    {
        Scene benchmark = new(Console.OpenStandardOutput());

        var title = new Label("Benchmarking");
        var progress = new Progress(0, 10).BottomOf(title, 0, Layouts.Alignment.Center);
        var iteration = new Label("Waiting").BottomOf(progress, 2, Layouts.Alignment.Center);
        benchmark.Manage(title, progress, iteration);
        benchmark.Flush();

        var sw = Stopwatch.StartNew();
        int iterations = 0;
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

        score.Text.Value = $"Score: {iterations}";
        results.Flush();

        Console.ReadKey();
    }
}