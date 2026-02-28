namespace RythmTester;

internal static class Entry
{
    public static void Run()
    {
        Console.Clear();
        Console.WriteLine("Press Any Key To Start");
        Console.ReadKey(intercept: true);
    }
}
