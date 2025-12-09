public static class GameData
{
    public static int score = 0;

    // True if we are loading FROM a save file (Continue button)
    public static bool startingFromSave = false;

    // Optional: also track if this intro has already run in this session
    public static bool hasSeenLevel1Intro = false;
}
