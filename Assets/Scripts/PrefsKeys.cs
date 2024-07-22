public static class PrefsKeys
{
    public const string UnlockedLevelKey = "UnlockedLevel";
    public const string ReachedLevelIndexKey = "ReachedLevelIndex";
    public const string TotalStarsKey = "TotalStars";

    public static string GetLevelStarsKey(int levelIndex)
    {
        return $"Level_{levelIndex}_Stars";
    }
}