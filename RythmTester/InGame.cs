using System.Diagnostics;

namespace RythmTester;

internal static class InGame
{
    public static void Run(GameState state)
    {
        ConsoleUi.EnsureConsoleSize(state.ResolutionWidth, state.ResolutionHeight);

        bool previousCursorVisible = Console.CursorVisible;
        Console.CursorVisible = false;

        try
        {
            RunOneButtonGameLoop(state);
        }
        finally
        {
            Console.CursorVisible = previousCursorVisible;
            Console.ResetColor();
            Console.SetCursorPosition(0, 0);
        }
    }

    private static void RunOneButtonGameLoop(GameState state)
    {
        List<BeatNote> beats = new();
        Stopwatch stopwatch = Stopwatch.StartNew();
        double beatMs = 60000.0 / state.Bpm;
        double nextBeatMs = 2000.0;
        double nextBeepMs = 2000.0;

        int score = 0;
        int combo = 0;
        int maxCombo = 0;
        double nextFrameAtMs = 0;

        string lastJudgeText = string.Empty;
        double lastJudgeEndMs = -1;

        while (true)
        {
            double nowMs = stopwatch.Elapsed.TotalMilliseconds;
            double travelTimeMs = state.NoteSpeed;
            int renderWidth = Math.Clamp(Console.WindowWidth - 1, 1, 160);
            int renderHeight = Math.Clamp(Console.WindowHeight, 1, 48);

            while (nowMs + travelTimeMs >= nextBeatMs)
            {
                beats.Add(new BeatNote(nextBeatMs));
                nextBeatMs += beatMs;
            }

            while (nowMs >= nextBeepMs)
            {
                QueueBeatBeep();
                nextBeepMs += beatMs;
            }

            while (Console.KeyAvailable)
            {
                ConsoleKey key = Console.ReadKey(intercept: true).Key;

                if (key == ConsoleKey.Escape)
                {
                    return;
                }

                if (TryJudgeBeat(beats, nowMs, state, ref score, ref combo, ref maxCombo, out string judgeText))
                {
                    lastJudgeText = judgeText;
                    lastJudgeEndMs = nowMs + 500;
                }
            }

            foreach (BeatNote beat in beats)
            {
                if (beat.Judged)
                {
                    continue;
                }

                if (nowMs - beat.TargetMs > state.MissJudge)
                {
                    beat.Judged = true;
                    combo = 0;
                    lastJudgeText = "MISS";
                    lastJudgeEndMs = nowMs + 500;
                }
            }

            beats.RemoveAll(beat => beat.Judged && nowMs - beat.TargetMs > 250);

            RenderOneButtonGame(
                beats,
                nowMs,
                score,
                combo,
                maxCombo,
                lastJudgeText,
                lastJudgeEndMs,
                travelTimeMs,
                renderWidth,
                renderHeight,
                state);

            double frameDurationMs = 1000.0 / Math.Clamp(state.Fps, 15, 240);
            if (nextFrameAtMs <= nowMs)
            {
                nextFrameAtMs = nowMs + frameDurationMs;
            }
            else
            {
                nextFrameAtMs += frameDurationMs;
            }

            WaitUntilFrame(stopwatch, nextFrameAtMs);
        }
    }

    private static bool TryJudgeBeat(
        List<BeatNote> beats,
        double nowMs,
        GameState state,
        ref int score,
        ref int combo,
        ref int maxCombo,
        out string judgeText)
    {
        judgeText = string.Empty;

        BeatNote? target = beats
            .Where(beat => !beat.Judged)
            .OrderBy(beat => Math.Abs(nowMs - beat.TargetMs))
            .FirstOrDefault();

        if (target is null)
        {
            return false;
        }

        double diff = Math.Abs(nowMs - target.TargetMs);
        if (diff > state.MissJudge)
        {
            return false;
        }

        target.Judged = true;
        int timingOffsetMs = (int)Math.Round(target.TargetMs - nowMs);
        string offsetText = FormatTimingOffset(timingOffsetMs);

        if (diff <= state.PerfectJudge)
        {
            combo++;
            maxCombo = Math.Max(maxCombo, combo);
            score += 1000 + combo * 3;
            judgeText = $"PERFECT {offsetText}";
            return true;
        }

        combo = 0;
        judgeText = $"MISS {offsetText}";
        return true;
    }

    private static void RenderOneButtonGame(
        List<BeatNote> beats,
        double nowMs,
        int score,
        int combo,
        int maxCombo,
        string lastJudgeText,
        double lastJudgeEndMs,
        double travelTimeMs,
        int width,
        int height,
        GameState state)
    {
        if (width < 40 || height < 10)
        {
            Console.SetCursorPosition(0, 0);
            Console.Write("Console window too small. Resize bigger (>= 40x10).");
            return;
        }

        int playTop = 4;
        int playBottom = height - 5;
        int judgeY = (playTop + playBottom) / 2;

        int leftStartX = 4;
        int rightStartX = width - 5;
        int judgeX = width / 2;

        char[][] grid = CreateGrid(width, height);

        DrawText(grid, 0, 0, $"Score: {score}   Combo: {combo}   Max Combo: {maxCombo}");
        DrawText(grid, Math.Max(0, width - 31), 0, $"BPM:{state.Bpm}  Speed:{state.NoteSpeed}  FPS:{state.Fps}");
        DrawText(grid, 0, 1, "ANY KEY: Hit Beat   ESC: Lobby");

        for (int x = leftStartX; x <= rightStartX; x++)
        {
            SetCell(grid, x, judgeY, '-');
        }

        for (int y = playTop; y <= playBottom; y++)
        {
            SetCell(grid, judgeX, y, '|');
        }

        foreach (BeatNote beat in beats)
        {
            if (beat.Judged)
            {
                continue;
            }

            double progress = (nowMs - (beat.TargetMs - travelTimeMs)) / travelTimeMs;
            if (progress < 0 || progress > 1.1)
            {
                continue;
            }

            int leftX = leftStartX + (int)Math.Round(progress * (judgeX - leftStartX));
            int rightX = rightStartX - (int)Math.Round(progress * (rightStartX - judgeX));

            leftX = Math.Clamp(leftX, leftStartX, judgeX);
            rightX = Math.Clamp(rightX, judgeX, rightStartX);

            SetCell(grid, leftX, judgeY, 'O');
            SetCell(grid, rightX, judgeY, 'O');
        }

        if (!string.IsNullOrWhiteSpace(lastJudgeText) && nowMs <= lastJudgeEndMs)
        {
            DrawText(grid, Math.Max(0, (width - lastJudgeText.Length) / 2), 2, lastJudgeText);
        }

        for (int y = 0; y < height; y++)
        {
            Console.SetCursorPosition(0, y);
            Console.Write(grid[y], 0, grid[y].Length);
        }
        Console.SetCursorPosition(0, 0);
    }

    private static char[][] CreateGrid(int width, int height)
    {
        char[][] grid = new char[height][];

        for (int y = 0; y < height; y++)
        {
            grid[y] = new string(' ', width).ToCharArray();
        }

        return grid;
    }

    private static void DrawText(char[][] grid, int x, int y, string text)
    {
        if (y < 0 || y >= grid.Length)
        {
            return;
        }

        for (int i = 0; i < text.Length; i++)
        {
            int drawX = x + i;
            if (drawX < 0 || drawX >= grid[y].Length)
            {
                continue;
            }

            grid[y][drawX] = text[i];
        }
    }

    private static void SetCell(char[][] grid, int x, int y, char c)
    {
        if (y < 0 || y >= grid.Length)
        {
            return;
        }

        if (x < 0 || x >= grid[y].Length)
        {
            return;
        }

        grid[y][x] = c;
    }

    private static double GetTravelTimeMs(int noteSpeed)
    {
        double normalized = (noteSpeed - 1) / 9.0;
        const double maxTravelMs = 4800;
        const double minTravelMs = 180;
        return maxTravelMs - ((maxTravelMs - minTravelMs) * normalized);
    }

    private static void QueueBeatBeep()
    {
        _ = Task.Run(PlayBeatBeep);
    }

    private static void PlayBeatBeep()
    {
        try
        {
            Console.Beep(1400, 20);
        }
        catch (PlatformNotSupportedException)
        {
            Console.Write("\a");
        }
    }

    private static void WaitUntilFrame(Stopwatch stopwatch, double targetFrameAtMs)
    {
        while (true)
        {
            double remainMs = targetFrameAtMs - stopwatch.Elapsed.TotalMilliseconds;
            if (remainMs <= 0)
            {
                return;
            }

            if (remainMs >= 20)
            {
                Thread.Sleep((int)(remainMs - 10));
                continue;
            }

            Thread.SpinWait(256);
        }
    }

    private static string FormatTimingOffset(int timingOffsetMs)
    {
        if (timingOffsetMs >= 0)
        {
            return $"+{timingOffsetMs}ms";
        }

        return $"{timingOffsetMs}ms";
    }

    private sealed class BeatNote
    {
        public double TargetMs { get; }
        public bool Judged { get; set; }

        public BeatNote(double targetMs)
        {
            TargetMs = targetMs;
            Judged = false;
        }
    }
}
