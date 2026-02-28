namespace RythmTester;

internal static class Settings
{
    public static void Run(GameState state)
    {
        int selectedIndex = 0;

        while (true)
        {
            Render(state, selectedIndex);

            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = Math.Min(4, selectedIndex + 1);
                    break;
                case ConsoleKey.LeftArrow:
                    ChangeValue(state, selectedIndex, -1);
                    break;
                case ConsoleKey.RightArrow:
                    ChangeValue(state, selectedIndex, 1);
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private static void Render(GameState state, int selectedIndex)
    {
        Console.Clear();
        Console.WriteLine($"{GetCursorPrefix(selectedIndex, 0)}Perfect Judge(ms): {state.PerfectJudge}");
        Console.WriteLine($"{GetCursorPrefix(selectedIndex, 1)}Miss Judge(ms): {state.MissJudge}");
        Console.WriteLine($"{GetCursorPrefix(selectedIndex, 2)}Note Speed(ms): {state.NoteSpeed}");
        Console.WriteLine($"{GetCursorPrefix(selectedIndex, 3)}BPM: {state.Bpm}");
        Console.WriteLine($"{GetCursorPrefix(selectedIndex, 4)}FPS: {state.Fps}");
        Console.WriteLine();
        Console.WriteLine("Up/Down: 커서 이동, Left/Right: 값 변경, Esc: 로비 복귀");
    }

    private static string GetCursorPrefix(int selectedIndex, int rowIndex)
    {
        return selectedIndex == rowIndex ? "> " : "  ";
    }

    private static void ChangeValue(GameState state, int selectedIndex, int delta)
    {
        switch (selectedIndex)
        {
            case 0:
                state.PerfectJudge = Math.Max(1, state.PerfectJudge + delta);
                state.MissJudge = Math.Max(state.MissJudge, state.PerfectJudge + 1);
                break;
            case 1:
                state.MissJudge = Math.Max(state.PerfectJudge + 1, state.MissJudge + delta);
                break;
            case 2:
                state.NoteSpeed = Math.Clamp(state.NoteSpeed + delta * 100, 100, 4800);
                break;
            case 3:
                state.Bpm = Math.Max(1, state.Bpm + delta);
                break;
            case 4:
                int[] fpsOptions = [10, 20, 30, 60, 144];
                int currentIndex = Array.IndexOf(fpsOptions, state.Fps);
                if (currentIndex < 0)
                {
                    currentIndex = 1;
                }

                if (delta < 0)
                {
                    currentIndex = Math.Max(0, currentIndex - 1);
                }
                else if (delta > 0)
                {
                    currentIndex = Math.Min(fpsOptions.Length - 1, currentIndex + 1);
                }

                state.Fps = fpsOptions[currentIndex];
                break;
        }
    }
}
