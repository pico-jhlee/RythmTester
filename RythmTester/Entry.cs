namespace RythmTester;

internal static class Entry
{
    public static void Run()
    {
        string[] lines =
        [
            "Press Any Key To Start"
        ];

        ConsoleUi.FitWindowToContent(lines);
        ConsoleUi.RenderFrame(lines);
        Console.ReadKey(intercept: true);
    }
}
