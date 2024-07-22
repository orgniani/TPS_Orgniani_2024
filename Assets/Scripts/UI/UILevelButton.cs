using UnityEngine;
using TMPro;

public class UILevelButton : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int levelIndex = 1;

    [Header("References")]
    [SerializeField] private GameObject lockImage;
    [SerializeField] private TMP_Text levelText;
    [SerializeField] private GameObject[] starFills;

    public int LevelIndex => levelIndex;

    public GameObject LockImage => lockImage;
    public GameObject LevelText => levelText.gameObject;

    public bool IsClickable { get; set; }

    public void Setup(int buttonNumber, int levelIndex)
    {
        this.levelIndex = levelIndex;

        int stars = LoadStars();
        DisplayStars(stars);

        levelText.text = $"{buttonNumber}";
    }

    private int LoadStars()
    {
        string key = PrefsKeys.GetLevelStarsKey(levelIndex);
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
