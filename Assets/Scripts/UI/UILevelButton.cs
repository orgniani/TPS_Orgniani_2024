using System.Runtime.CompilerServices;
using UnityEngine;

public class UILevelButton : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int levelIndex = 1;

    [Header("References")]
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject levelText;
    [SerializeField] private GameObject[] starFills;

    public int LevelIndex => levelIndex;

    public GameObject LockImage => lockImage;
    public GameObject LevelText => levelText;

    public bool IsClickable { get; set; }

    private void OnEnable()
    {
        int stars = LoadStars();
        DisplayStars(stars);
    }

    private int LoadStars()
    {
        string key = $"Level_{levelIndex}_Stars";
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
