public static class PrefsKeys
{
    public const string UnlockedLevelKey = "UnlockedLevel";
    public const string ReachedLevelIndexKey = "ReachedLevelIndex";

    public static string GetLevelStarsKey(int levelIndex)
    {
        return $"Level_{levelIndex}_Stars";
    }
}