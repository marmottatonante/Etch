namespace Etch;

public static class Tests
{
    public static void Benchmark(int iterations = 100000)
    {
        double deltaSum = 0;
        double lowestDelta = double.MaxValue;
        double highestDelta = double.MinValue;

        Renderer renderer = new();

        // Centered FPS counter
        renderer.Root = new Binder(() => $"FPS: {1 / renderer.FrameTime}");

        // JIT warm up
        for(int i = 0; i < 100; i++)
            renderer.RenderOnce();
        
        // Benchmark
        for(int i = 0; i < iterations; i++)
        {
            renderer.RenderOnce();
            deltaSum += renderer.DeltaTime;
            if(renderer.DeltaTime < lowestDelta) lowestDelta = renderer.DeltaTime;
            if(renderer.DeltaTime > highestDelta) highestDelta = renderer.DeltaTime;
        }

        double average = deltaSum / iterations;
        Console.WriteLine("Performance measurement finished.");
        Console.WriteLine($"Average FPS: {1 / average}");
        Console.WriteLine($"Lowest FPS: {1 / highestDelta}");
        Console.WriteLine($"Highest FPS: {1 / lowestDelta}");
    }
}