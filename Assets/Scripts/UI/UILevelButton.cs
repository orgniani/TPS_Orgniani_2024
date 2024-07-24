using UnityEngine;
using TMPro;

public class UILevelButton : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject lockImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject[] starFills;

    public GameObject LockImage => lockImage;
    public GameObject LevelText => levelText.gameObject;

    public int LevelIndex { get; set; }

    public bool IsClickable { get; set; }

    public bool IsAvailable { get; set; }

    public void Setup(int buttonNumber, int levelIndex)
    {
        LevelIndex = levelIndex;

        int stars = LoadStars();
        DisplayStars(stars);

        levelText.text = $"{buttonNumber}";
    }

    private int LoadStars()
    {
        string key = PrefsKeys.GetLevelStarsKey(LevelIndex);
        return PlayerPrefs.GetInt(key, 0);
    }

    private void DisplayStars(int stars)
    {
        for (int i = 0; i < starFills.Length; i++)
        {
            starFills[i].SetActive(i < stars);
        }
    }
}
