using Etch.UI;
using Pith.Reactive;
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
        Shell.AlternateBuffer = true;
        Shell.CursorVisible = false;

        Property<double> current = new(0);

        Shell.Root = new Center(
            new Stack(Direction.Vertical, Alignment.Center, 2,
                new Image(Figlet.Split('\n')),
                new Stack(Direction.Vertical, Alignment.Center, 0,
                    new Label("Benchmarking"),
                    new Progress(0, 10, current))));
        Shell.Render();

        Shell.Metrics.Active = true;
        var sw = Stopwatch.StartNew();
        double totalDraw = 0;
        double totalFlush = 0;

        int iterations = 0;
        while (sw.Elapsed.TotalSeconds < 10)
        {
            current.Value = sw.Elapsed.TotalSeconds;
            Shell.Render();

            totalDraw += Shell.Metrics.DrawTime;
            totalFlush += Shell.Metrics.FlushTime;
            iterations++;
        }

        sw.Stop();
        Shell.Metrics.Active = false;

        static string Ms(double seconds) => $"{seconds * 1000:F3}ms";
        static string Sec(double seconds) => $"{seconds:F3}s";
        static string Fps(double seconds) => $"{1.0 / seconds:F0}fps";

        double avgDraw = totalDraw / iterations;
        double avgFlush = totalFlush / iterations;
        double avgDelta = avgDraw + avgFlush;

        Shell.Clear();
        Shell.Root = new Center(
            new Stack(Direction.Vertical, Alignment.Center, 1,
                new Image(Figlet.Split('\n')),
                new Stack(Direction.Vertical, Alignment.Start, 0,
                    new Stack(Direction.Horizontal, Alignment.Start, 2, [new Label(""),          new Label("Total"),                     new Label("Avg"),        new Label("Avg FPS")]),
                    new Stack(Direction.Horizontal, Alignment.Start, 2, [new Label("Draw"),      new Label(Sec(totalDraw)),              new Label(Ms(avgDraw)),  new Label(Fps(avgDraw))  ]),
                    new Stack(Direction.Horizontal, Alignment.Start, 2, [new Label("Flush"),     new Label(Sec(totalFlush)),             new Label(Ms(avgFlush)), new Label(Fps(avgFlush)) ]),
                    new Stack(Direction.Horizontal, Alignment.Start, 2, [new Label("Delta"),     new Label(Sec(totalDraw + totalFlush)), new Label(Ms(avgDelta)), new Label(Fps(avgDelta)) ]),
                new Label($"Score: {iterations}"),
                new Label("Press any key to continue"))));
        Shell.Render();
        Console.ReadKey();

        Shell.AlternateBuffer = false;
        Shell.CursorVisible = true;
    }
}