namespace RythmTester;

internal static class Entry
{
    public static void Run(GameState state)
    {
        string[] lines =
        [
            "Press Any Key To Start"
        ];

        ConsoleUi.EnsureConsoleSize(state.ResolutionWidth, state.ResolutionHeight);
        ConsoleUi.RenderFrame(lines);
        Console.ReadKey(intercept: true);
        ConsoleSound.QueueSelectionBeep();
    }
}
