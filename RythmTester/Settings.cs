namespace RythmTester;

internal static class Settings
{
    private enum SettingsTab
    {
        System,
        LevelDesign
    }

    private static readonly (int Width, int Height)[] ResolutionPresets =
    [
        (100, 30),
        (160, 48)
    ];

    private static readonly int[] FpsOptions = [10, 20, 30, 60, 75, 90, 120, 144, 165, 240, 360, 400];

    public static void Run(GameState state)
    {
        SettingsTab activeTab = SettingsTab.System;
        int systemSelectedIndex = 0;
        int levelDesignSelectedIndex = 0;

        while (true)
        {
            Render(state, activeTab, systemSelectedIndex, levelDesignSelectedIndex);

            ConsoleKeyInfo keyInfo = Console.ReadKey(intercept: true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow:
                    if (activeTab == SettingsTab.System)
                    {
                        systemSelectedIndex = Math.Max(0, systemSelectedIndex - 1);
                    }
                    else
                    {
                        levelDesignSelectedIndex = Math.Max(0, levelDesignSelectedIndex - 1);
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (activeTab == SettingsTab.System)
                    {
                        systemSelectedIndex = Math.Min(1, systemSelectedIndex + 1);
                    }
                    else
                    {
                        levelDesignSelectedIndex = Math.Min(3, levelDesignSelectedIndex + 1);
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    ChangeValue(state, activeTab, activeTab == SettingsTab.System ? systemSelectedIndex : levelDesignSelectedIndex, -1);
                    break;
                case ConsoleKey.RightArrow:
                    ChangeValue(state, activeTab, activeTab == SettingsTab.System ? systemSelectedIndex : levelDesignSelectedIndex, 1);
                    break;
                case ConsoleKey.Tab:
                    activeTab = activeTab == SettingsTab.System ? SettingsTab.LevelDesign : SettingsTab.System;
                    break;
                case ConsoleKey.Escape:
                    return;
            }
        }
    }

    private static void Render(GameState state, SettingsTab activeTab, int systemSelectedIndex, int levelDesignSelectedIndex)
    {
        string tabLine = activeTab == SettingsTab.System
            ? "[System] | Level Design"
            : "System | [Level Design]";

        string[] settingLines = activeTab == SettingsTab.System
            ?
            [
                $"{GetCursorPrefix(systemSelectedIndex, 0)}Resolution: {state.ResolutionWidth} X {state.ResolutionHeight}",
                $"{GetCursorPrefix(systemSelectedIndex, 1)}FPS: {state.Fps}"
            ]
            :
            [
                $"{GetCursorPrefix(levelDesignSelectedIndex, 0)}Note Speed: {state.NoteSpeed}",
                $"{GetCursorPrefix(levelDesignSelectedIndex, 1)}BPM: {state.Bpm}",
                $"{GetCursorPrefix(levelDesignSelectedIndex, 2)}Perfect Judge(ms): {state.PerfectJudge}",
                $"{GetCursorPrefix(levelDesignSelectedIndex, 3)}Miss Judge(ms): {state.MissJudge}"
            ];

        string[] lines =
        [
            tabLine,
            string.Empty,
            ..settingLines,
            string.Empty,
            "Tab: 탭 전환, Up/Down: 커서 이동, Left/Right: 값 변경, Esc: 로비 복귀"
        ];

        ConsoleUi.FitWindowToContent(lines);
        ConsoleUi.RenderFrame(lines);
    }

    private static string GetCursorPrefix(int selectedIndex, int rowIndex)
    {
        return selectedIndex == rowIndex ? "> " : "  ";
    }

    private static void ChangeValue(GameState state, SettingsTab activeTab, int selectedIndex, int delta)
    {
        if (activeTab == SettingsTab.System)
        {
            ChangeSystemValue(state, selectedIndex, delta);
            return;
        }

        ChangeLevelDesignValue(state, selectedIndex, delta);
    }

    private static void ChangeSystemValue(GameState state, int selectedIndex, int delta)
    {
        switch (selectedIndex)
        {
            case 0:
                int presetIndex = FindResolutionPresetIndex(state.ResolutionWidth, state.ResolutionHeight);

                if (delta < 0)
                {
                    presetIndex = Math.Max(0, presetIndex - 1);
                }
                else if (delta > 0)
                {
                    presetIndex = Math.Min(ResolutionPresets.Length - 1, presetIndex + 1);
                }

                (state.ResolutionWidth, state.ResolutionHeight) = ResolutionPresets[presetIndex];
                break;
            case 1:
                int currentIndex = FindFpsOptionIndex(state.Fps);

                if (delta < 0)
                {
                    currentIndex = Math.Max(0, currentIndex - 1);
                }
                else if (delta > 0)
                {
                    currentIndex = Math.Min(FpsOptions.Length - 1, currentIndex + 1);
                }

                state.Fps = FpsOptions[currentIndex];
                break;
        }
    }

    private static void ChangeLevelDesignValue(GameState state, int selectedIndex, int delta)
    {
        switch (selectedIndex)
        {
            case 0:
                state.NoteSpeed = Math.Clamp(state.NoteSpeed + delta, 1, 10);
                break;
            case 1:
                state.Bpm = Math.Max(1, state.Bpm + delta);
                break;
            case 2:
                state.PerfectJudge = Math.Max(1, state.PerfectJudge + delta);
                state.MissJudge = Math.Max(state.MissJudge, state.PerfectJudge + 1);
                break;
            case 3:
                state.MissJudge = Math.Max(state.PerfectJudge + 1, state.MissJudge + delta);
                break;
        }
    }

    private static int FindResolutionPresetIndex(int width, int height)
    {
        int index = Array.FindIndex(ResolutionPresets, preset => preset.Width == width && preset.Height == height);
        return index < 0 ? 0 : index;
    }

    private static int FindFpsOptionIndex(int expect)
    {
        int index = Array.FindIndex(FpsOptions, fps => fps == expect);
        return index < 0 ? 0 : index;
    }
}
