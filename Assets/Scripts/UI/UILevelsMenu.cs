using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UILevelsMenu : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject menuScreen;
    [SerializeField] private UIIndividualLevelMenu levelCanvasPrefab;
    [SerializeField] private GameObject loadingScreen;
    [SerializeField] private GameObject comingSoonScreen;

    [Header("Text")]
    [SerializeField] private TMP_Text starsCountText;

    [Header("References")]
    [SerializeField] private LevelManager levelManager;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private EventSystem eventSystem;

    [Header("Parameters")]
    [SerializeField] private int totalLevels = 3;
    [SerializeField] private int currentAvailableLevels = 3;

    [SerializeField] private int maxButtonsPerCanvas = 9;
    [SerializeField] private int maxStarsPerLevel = 3;

    [SerializeField] private float switchPageCooldown = 1f;

    private List<UIIndividualLevelMenu> levelCanvases = new List<UIIndividualLevelMenu>();

    private int currentLevelsCanvasIndex = 1;

    private bool canSwitchPage = true;

    public int CurrentAvailableLevels => currentAvailableLevels;

    private void OnEnable()
    {
        UpdateStarsCount();

        if(levelCanvases.Count == 0) CreateLevelMenus();

        menuScreen.SetActive(true);

        PrefsKeys.InitializeUnlockedLevelKey(levelManager.NextLevelBuildIndex);
        int unlockedLevel = PlayerPrefs.GetInt(PrefsKeys.UnlockedLevelKey);

        currentLevelsCanvasIndex = GetCanvasIndexForLevel(unlockedLevel);

        UpdateLevelCanvas(currentLevelsCanvasIndex);
    }


    public void StartSpecificLevel(UILevelButton levelButton, int levelIndex)
    {
        if (!levelButton.IsAvailable)
        {
            comingSoonScreen.SetActive(true);
            return;
        }

        if (!levelButton.IsClickable) return;

        menuManager.StartSpecificLevel(loadingScreen, levelIndex);
    }

    public void OnNextPage()
    {
        if (!canSwitchPage) return;

        SwitchPage(currentLevelsCanvasIndex + 1);
    }


    public void OnBackPage()
    {
        if (!canSwitchPage) return;

        SwitchPage(currentLevelsCanvasIndex - 1);
    }

    public void OnBackToMenu()
    {
        menuManager.CloseScreen(menuScreen);

        foreach (var canvas in levelCanvases)
        {
            if (!canvas.gameObject.activeSelf) continue;
            menuManager.CloseScreen(canvas.gameObject);
        }

        menuManager.CloseScreen(gameObject);
    }

    private void CreateLevelMenus()
    {
        int levelsPerCanvas = 9;
        int totalCanvases = Mathf.CeilToInt((float)totalLevels / levelsPerCanvas);

        for (int i = 0; i < totalCanvases; i++)
        {
            if (levelCanvasPrefab == null)
            {
                Debug.LogError("Level Canvas Prefab is not assigned.");
                return;
            }

            UIIndividualLevelMenu individualLevelMenu = Instantiate(levelCanvasPrefab, transform);
            levelCanvases.Add(individualLevelMenu);

            int startLevel = i * levelsPerCanvas + levelManager.NextLevelBuildIndex;
            int endLevel = Mathf.Min((i + 1) * levelsPerCanvas, totalLevels);

            individualLevelMenu.Setup(startLevel, endLevel, eventSystem, this);
        }
    }

    private void SwitchPage(int newIndex)
    {
        if (newIndex < 0 || newIndex >= levelCanvases.Count) return;

        menuManager.CloseScreen(levelCanvases[currentLevelsCanvasIndex].gameObject);

        currentLevelsCanvasIndex = newIndex;
        UpdateLevelCanvas(currentLevelsCanvasIndex);

        StartCoroutine(WaitToSwitchPage());
    }

    private void UpdateLevelCanvas(int index)
    {
        if (index < 0 || index >= levelCanvases.Count)
        {
            Debug.LogError($"Invalid index {index} for UpdateLevelCanvas.");
            return;
        }

        levelCanvases[index].gameObject.SetActive(true);
        levelCanvases[index].UpdateButtons();
    }

    private void UpdateStarsCount()
    {
        int maxStars = totalLevels * maxStarsPerLevel;

        int totalStars = PlayerPrefs.GetInt(PrefsKeys.TotalStarsKey, 0);
        starsCountText.text = $"{totalStars}/{maxStars}";
    }

    /// <summary>
    /// The canvas that will open when the play button is pressed will always be 
    /// the canvas containing the button for the last unlocked level.
    /// </summary>
    private int GetCanvasIndexForLevel(int levelSceneIndex)
    {
        int levelsPerCanvas = maxButtonsPerCanvas;
        int canvasIndex = ((levelSceneIndex - levelManager.NextLevelBuildIndex) / levelsPerCanvas);

        return Mathf.Clamp(canvasIndex, 0, levelCanvases.Count - 1);
    }

    private IEnumerator WaitToSwitchPage()
    {
        canSwitchPage = false;

        yield return new WaitForSeconds(switchPageCooldown);

        canSwitchPage = true;
    }
}
