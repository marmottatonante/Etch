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
        var subtitle = new Label("World").Under(title);
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
        var progress = new Progress(0, 10).Under(title, alignment: Layouts.Alignment.Center);
        benchmark.Add(title).Add(progress).Render();
        benchmark.Metrics.Active = true;

        var sw = Stopwatch.StartNew();
        double totalDraw = 0;
        double totalFlush = 0;

        int iterations = 0;
        while (sw.Elapsed.TotalSeconds < 10)
        {
            progress.Current.Value = sw.Elapsed.TotalSeconds;
            benchmark.Render();

            totalDraw += benchmark.Metrics.DrawTime;
            totalFlush += benchmark.Metrics.FlushTime;
            iterations++;
        }

        sw.Stop();

        static string Ms(double seconds) => $"{seconds * 1000:F3}ms";
        static string Sec(double seconds) => $"{seconds:F3}s";
        static string Fps(double seconds) => $"{1.0 / seconds:F0}fps";

        double avgDraw = totalDraw / iterations;
        double avgFlush = totalFlush / iterations;
        double avgDelta = avgDraw + avgFlush;

        Console.Clear();
        Console.WriteLine($"Average Draw: {Ms(avgDraw)} - Average Flush: {Ms(avgFlush)} - Average Delta: {Ms(avgDelta)}");
        Console.WriteLine($"Total Draw: {Sec(totalDraw)} - Total Flush: {Sec(totalFlush)} - Total Delta: {Sec(totalDraw + totalFlush)}");
        Console.WriteLine($"Score: {iterations}");
    }
}