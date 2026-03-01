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
                        int nextIndex = Math.Max(0, systemSelectedIndex - 1);
                        if (nextIndex != systemSelectedIndex)
                        {
                            systemSelectedIndex = nextIndex;
                            ConsoleSound.QueueSelectionBeep();
                        }
                    }
                    else
                    {
                        int nextIndex = Math.Max(0, levelDesignSelectedIndex - 1);
                        if (nextIndex != levelDesignSelectedIndex)
                        {
                            levelDesignSelectedIndex = nextIndex;
                            ConsoleSound.QueueSelectionBeep();
                        }
                    }
                    break;
                case ConsoleKey.DownArrow:
                    if (activeTab == SettingsTab.System)
                    {
                        int nextIndex = Math.Min(1, systemSelectedIndex + 1);
                        if (nextIndex != systemSelectedIndex)
                        {
                            systemSelectedIndex = nextIndex;
                            ConsoleSound.QueueSelectionBeep();
                        }
                    }
                    else
                    {
                        int nextIndex = Math.Min(3, levelDesignSelectedIndex + 1);
                        if (nextIndex != levelDesignSelectedIndex)
                        {
                            levelDesignSelectedIndex = nextIndex;
                            ConsoleSound.QueueSelectionBeep();
                        }
                    }
                    break;
                case ConsoleKey.LeftArrow:
                    if (ChangeValue(state, activeTab, activeTab == SettingsTab.System ? systemSelectedIndex : levelDesignSelectedIndex, -1))
                    {
                        ConsoleSound.QueueSelectionBeep();
                    }
                    break;
                case ConsoleKey.RightArrow:
                    if (ChangeValue(state, activeTab, activeTab == SettingsTab.System ? systemSelectedIndex : levelDesignSelectedIndex, 1))
                    {
                        ConsoleSound.QueueSelectionBeep();
                    }
                    break;
                case ConsoleKey.Tab:
                    ConsoleSound.QueueSelectionBeep();
                    activeTab = activeTab == SettingsTab.System ? SettingsTab.LevelDesign : SettingsTab.System;
                    break;
                case ConsoleKey.Escape:
                    ConsoleSound.QueueSelectionBeep();
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

        ConsoleUi.EnsureConsoleSize(state.ResolutionWidth, state.ResolutionHeight);
        ConsoleUi.RenderFrame(lines);
    }

    private static string GetCursorPrefix(int selectedIndex, int rowIndex)
    {
        return selectedIndex == rowIndex ? "> " : "  ";
    }

    private static bool ChangeValue(GameState state, SettingsTab activeTab, int selectedIndex, int delta)
    {
        if (activeTab == SettingsTab.System)
        {
            return ChangeSystemValue(state, selectedIndex, delta);
        }

        return ChangeLevelDesignValue(state, selectedIndex, delta);
    }

    private static bool ChangeSystemValue(GameState state, int selectedIndex, int delta)
    {
        switch (selectedIndex)
        {
            case 0:
                int previousWidth = state.ResolutionWidth;
                int previousHeight = state.ResolutionHeight;
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
                return state.ResolutionWidth != previousWidth || state.ResolutionHeight != previousHeight;
            case 1:
                int previousFps = state.Fps;
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
                return state.Fps != previousFps;
        }

        return false;
    }

    private static bool ChangeLevelDesignValue(GameState state, int selectedIndex, int delta)
    {
        switch (selectedIndex)
        {
            case 0:
                int previousNoteSpeed = state.NoteSpeed;
                state.NoteSpeed = Math.Clamp(state.NoteSpeed + delta, 1, 20);
                return state.NoteSpeed != previousNoteSpeed;
            case 1:
                int previousBpm = state.Bpm;
                state.Bpm = Math.Max(1, state.Bpm + delta);
                return state.Bpm != previousBpm;
            case 2:
                int previousPerfectJudge = state.PerfectJudge;
                int previousMissJudgeForPerfectChange = state.MissJudge;
                state.PerfectJudge = Math.Max(1, state.PerfectJudge + delta);
                state.MissJudge = Math.Max(state.MissJudge, state.PerfectJudge + 1);
                return state.PerfectJudge != previousPerfectJudge || state.MissJudge != previousMissJudgeForPerfectChange;
            case 3:
                int previousMissJudge = state.MissJudge;
                state.MissJudge = Math.Max(state.PerfectJudge + 1, state.MissJudge + delta);
                return state.MissJudge != previousMissJudge;
        }

        return false;
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
