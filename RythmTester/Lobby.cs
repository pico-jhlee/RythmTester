namespace RythmTester;

internal static class Lobby
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
                    int upNextIndex = Math.Max(0, selectedIndex - 1);
                    if (upNextIndex != selectedIndex)
                    {
                        selectedIndex = upNextIndex;
                        ConsoleSound.QueueSelectionBeep();
                    }
                    break;
                case ConsoleKey.DownArrow:
                    int downNextIndex = Math.Min(2, selectedIndex + 1);
                    if (downNextIndex != selectedIndex)
                    {
                        selectedIndex = downNextIndex;
                        ConsoleSound.QueueSelectionBeep();
                    }
                    break;
                case ConsoleKey.Enter:
                    ConsoleSound.QueueSelectionBeep();
                    if (HandleSelection(state, selectedIndex))
                    {
                        return;
                    }
                    break;
                case ConsoleKey.Escape:
                    if (selectedIndex != 2)
                    {
                        selectedIndex = 2;
                        ConsoleSound.QueueSelectionBeep();
                    }
                    break;
            }
        }
    }

    private static void Render(GameState state, int selectedIndex)
    {
        string[] lines =
        [
            $"{GetCursorPrefix(selectedIndex, 0)}Game Start",
            $"{GetCursorPrefix(selectedIndex, 1)}Settings",
            $"{GetCursorPrefix(selectedIndex, 2)}Exit",
            string.Empty,
            "Up/Down: 커서 이동, Enter: 선택"
        ];

        ConsoleUi.EnsureConsoleSize(state.ResolutionWidth, state.ResolutionHeight);
        ConsoleUi.RenderFrame(lines);
    }

    private static string GetCursorPrefix(int selectedIndex, int rowIndex)
    {
        return selectedIndex == rowIndex ? "> " : "  ";
    }

    private static bool HandleSelection(GameState state, int selectedIndex)
    {
        switch (selectedIndex)
        {
            case 0:
                InGame.Run(state);
                return false;
            case 1:
                Settings.Run(state);
                return false;
            case 2:
                return true;
            default:
                return false;
        }
    }
}
