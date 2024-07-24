using UnityEngine;

public static class PrefsKeys
{
    public const string UnlockedLevelKey = "UnlockedLevel";
    public const string ReachedLevelIndexKey = "ReachedLevelIndex";
    public const string TotalStarsKey = "TotalStars";
    public const string VolumeKey = "MusicVolume";


    public static string GetLevelStarsKey(int levelIndex)
    {
        return $"Level_{levelIndex}_Stars";
    }

    public static void InitializeUnlockedLevelKey(int firstLevelIndex)
    {
        if (PlayerPrefs.HasKey(UnlockedLevelKey)) return;
        PlayerPrefs.SetInt(UnlockedLevelKey, firstLevelIndex);
    }
}