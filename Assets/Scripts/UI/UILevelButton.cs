using UnityEngine;

public class UILevelButton : MonoBehaviour
{
    [Header("Parameters")]
    [SerializeField] private int levelIndex = 1;

    [Header("References")]
    [SerializeField] private GameObject lockImage;
    [SerializeField] private GameObject levelText;

    public int LevelIndex => levelIndex;

    public GameObject LockImage => lockImage;
    public GameObject LevelText => levelText;

    public bool IsClickable { get; set; }
}
