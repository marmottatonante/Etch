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
        Scene helloWorld = new(Console.OpenStandardOutput(),
                               (Console.WindowWidth, Console.WindowHeight));

        var title = new Label("Hello");
        var subtitle = new Label("World").BottomOf(title);
        helloWorld.Add(title).Add(subtitle).Render();

        Console.ReadKey();

        title.Position.Value += (0, 1);
        helloWorld.Render();

        Console.ReadKey();
    }

    public static void Benchmark()
    {
        Scene benchmark = new(Console.OpenStandardOutput(),
                              (Console.WindowWidth, Console.WindowHeight));

        var title = new Label("Benchmarking");
        var progress = new Progress(0, 10).BottomOf(title, 0, Layouts.Alignment.Center);
        var iteration = new Label("Waiting").BottomOf(progress, 2, Layouts.Alignment.Center);
        benchmark.Add(title).Add(progress).Add(iteration).Render();

        benchmark.Metrics.Active = true;
        var sw = Stopwatch.StartNew();
        int iterations = 0;
        while (sw.Elapsed.TotalSeconds < 10)
        {
            progress.Current.Value = sw.Elapsed.TotalSeconds;
            iteration.Text.Value = iterations.ToString();
            benchmark.Render();
            iterations++;
        }
        benchmark.Metrics.Active = false;
        sw.Stop();

        Console.Clear();

        static string Ms(double seconds) => $"{seconds * 1000:F4}ms";
        static string Sec(double seconds) => $"{seconds:F3}s";
        static string Fps(double seconds) => $"{1.0 / seconds:F0}fps";

        Scene results = new(Console.OpenStandardOutput(), 
                            (Console.WindowWidth, Console.WindowHeight));

        // Header row
        var h0 = new Label("");
        var h1 = new Label("Total").RightOf(h0);
        var h2 = new Label("Avg").RightOf(h1, 2);
        var h3 = new Label("Avg FPS").RightOf(h2, 2);

        // Draw row
        var d0 = new Label("Draw").BottomOf(h0);
        var d1 = new Label(Sec(benchmark.Metrics.TotalDraw)).RightOf(d0, 2);
        var d2 = new Label(Ms(benchmark.Metrics.AverageDraw)).RightOf(d1, 2);
        var d3 = new Label(Fps(benchmark.Metrics.AverageDraw)).RightOf(d2, 2);

        // Flush row
        var f0 = new Label("Flush").BottomOf(d0);
        var f1 = new Label(Sec(benchmark.Metrics.TotalFlush)).RightOf(f0, 2);
        var f2 = new Label(Ms(benchmark.Metrics.AverageFlush)).RightOf(f1, 2);
        var f3 = new Label(Fps(benchmark.Metrics.AverageFlush)).RightOf(f2, 2);

        // Delta row
        var dl0 = new Label("Delta").BottomOf(f0);
        var dl1 = new Label(Sec(benchmark.Metrics.TotalDelta)).RightOf(dl0, 2);
        var dl2 = new Label(Ms(benchmark.Metrics.AverageDelta)).RightOf(dl1, 2);
        var dl3 = new Label(Fps(benchmark.Metrics.AverageDelta)).RightOf(dl2, 2);

        var score = new Label($"Score: {iterations}").BottomOf(dl0, 1);

        results.Add(h0).Add(h1).Add(h2).Add(h3)
               .Add(d0).Add(d1).Add(d2).Add(d3)
               .Add(f0).Add(f1).Add(f2).Add(f3)
               .Add(dl0).Add(dl1).Add(dl2).Add(dl3)
               .Add(score)
               .Render();

        Console.ReadKey();
    }
}