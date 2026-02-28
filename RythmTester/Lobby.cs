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
        Console.Clear();
        Console.WriteLine($"{GetCursorPrefix(selectedIndex, 0)}Game Start");
        Console.WriteLine($"{GetCursorPrefix(selectedIndex, 1)}Settings");
        Console.WriteLine($"{GetCursorPrefix(selectedIndex, 2)}Exit");
        Console.WriteLine();
        Console.WriteLine("Up/Down: 커서 이동, Enter: 선택");
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
                Console.Clear();
                Console.WriteLine("게임을 종료합니다.");
                return true;
            default:
                return false;
        }
    }
}
