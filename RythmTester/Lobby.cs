namespace RythmTester;

internal static class Lobby
{
    public static void Run(GameState state)
    {
        int selectedIndex = 0;

        while (true)
        {
            Render(selectedIndex);

            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    selectedIndex = Math.Max(0, selectedIndex - 1);
                    break;
                case ConsoleKey.DownArrow:
                    selectedIndex = Math.Min(2, selectedIndex + 1);
                    break;
                case ConsoleKey.Enter:
                    if (HandleSelection(state, selectedIndex))
                    {
                        return;
                    }
                    break;
                case ConsoleKey.Escape:
                    selectedIndex = 2;
                    break;
            }
        }
    }

    private static void Render(int selectedIndex)
    {
        string[] lines =
        [
            $"{GetCursorPrefix(selectedIndex, 0)}Game Start",
            $"{GetCursorPrefix(selectedIndex, 1)}Settings",
            $"{GetCursorPrefix(selectedIndex, 2)}Exit",
            string.Empty,
            "Up/Down: 커서 이동, Enter: 선택"
        ];

        ConsoleUi.FitWindowToContent(lines);
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
                string[] exitLines = ["게임을 종료합니다."];
                ConsoleUi.FitWindowToContent(exitLines);
                ConsoleUi.RenderFrame(exitLines);
                return true;
            default:
                return false;
        }
    }
}
