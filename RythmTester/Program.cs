namespace RythmTester;

internal class Program
{
    static void Main(string[] args)
    {
        GameState state = new();

        Entry.Run(state);
        Lobby.Run(state);
    }
}

internal sealed class GameState
{
    public int PerfectJudge { get; set; } = 53;
    public int MissJudge { get; set; } = 90;
    public int NoteSpeed { get; set; } = 5;
    public int Bpm { get; set; } = 60;
    public int Fps { get; set; } = 60;
    public int ResolutionWidth { get; set; } = 100;
    public int ResolutionHeight { get; set; } = 30;
}
