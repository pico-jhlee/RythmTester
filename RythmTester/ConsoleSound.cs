namespace RythmTester;

internal static class ConsoleSound
{
    public static void QueueBeatBeep()
    {
        QueueBeep(1400, 20);
    }

    public static void QueueSelectionBeep()
    {
        QueueBeep(1400, 20);
    }

    private static void QueueBeep(int frequency, int durationMs)
    {
        _ = Task.Run(() => PlayBeep(frequency, durationMs));
    }

    private static void PlayBeep(int frequency, int durationMs)
    {
        try
        {
            Console.Beep(frequency, durationMs);
        }
        catch (PlatformNotSupportedException)
        {
            Console.Write("\a");
        }
    }
}
