namespace RythmTester;

internal static class ConsoleUi
{
    public static void FitWindowToContent(params string[] lines)
    {
        int contentWidth = 1;
        for (int i = 0; i < lines.Length; i++)
        {
            contentWidth = Math.Max(contentWidth, GetDisplayWidth(lines[i]));
        }

        EnsureConsoleSize(contentWidth + 2, lines.Length + 2);
    }

    public static void EnsureConsoleSize(int width, int height)
    {
        if (Console.IsOutputRedirected)
        {
            return;
        }

        try
        {
            int targetWidth = Math.Clamp(width, 1, Console.LargestWindowWidth);
            int targetHeight = Math.Clamp(height, 1, Console.LargestWindowHeight);

            if (Console.WindowWidth > targetWidth || Console.WindowHeight > targetHeight)
            {
                Console.SetWindowSize(targetWidth, targetHeight);
            }

            if (Console.BufferWidth != targetWidth || Console.BufferHeight != targetHeight)
            {
                Console.SetBufferSize(targetWidth, targetHeight);
            }

            if (Console.WindowWidth != targetWidth || Console.WindowHeight != targetHeight)
            {
                Console.SetWindowSize(targetWidth, targetHeight);
            }

            Console.SetCursorPosition(0, 0);
        }
        catch (IOException)
        {
        }
        catch (ArgumentOutOfRangeException)
        {
        }
        catch (PlatformNotSupportedException)
        {
        }
    }

    public static void RenderFrame(params string[] lines)
    {
        if (Console.IsOutputRedirected)
        {
            Console.WriteLine(string.Join(Environment.NewLine, lines));
            return;
        }

        int width = Math.Max(1, Console.WindowWidth - 1);
        int height = Math.Max(1, Console.WindowHeight);

        for (int y = 0; y < height; y++)
        {
            Console.SetCursorPosition(0, y);
            string line = y < lines.Length ? lines[y] : string.Empty;
            WritePaddedToWidth(line, width);
        }
        Console.SetCursorPosition(0, 0);
    }

    private static void WritePaddedToWidth(string line, int width)
    {
        int used = 0;
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            int cellWidth = GetCellWidth(c);
            if (used + cellWidth > width)
            {
                break;
            }

            Console.Write(c);
            used += cellWidth;
        }

        if (used < width)
        {
            Console.Write(new string(' ', width - used));
        }
    }

    private static int GetDisplayWidth(string text)
    {
        int width = 0;
        for (int i = 0; i < text.Length; i++)
        {
            width += GetCellWidth(text[i]);
        }

        return width;
    }

    private static int GetCellWidth(char c)
    {
        if (char.IsControl(c))
        {
            return 0;
        }

        if (c <= 0x7F)
        {
            return 1;
        }

        return IsWideChar(c) ? 2 : 1;
    }

    private static bool IsWideChar(char c)
    {
        return
            (c >= '\u1100' && c <= '\u115F') ||
            (c >= '\u2329' && c <= '\u232A') ||
            (c >= '\u2E80' && c <= '\uA4CF') ||
            (c >= '\uAC00' && c <= '\uD7A3') ||
            (c >= '\uF900' && c <= '\uFAFF') ||
            (c >= '\uFE10' && c <= '\uFE19') ||
            (c >= '\uFE30' && c <= '\uFE6F') ||
            (c >= '\uFF00' && c <= '\uFF60') ||
            (c >= '\uFFE0' && c <= '\uFFE6');
    }
}
