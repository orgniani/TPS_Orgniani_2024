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

    [Header("References")]
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private TMP_Text starsCountText;
    [SerializeField] private EventSystem eventSystem;

    [Header("Parameters")]
    [SerializeField] private int totalLevels = 3;
    [SerializeField] private float switchPageCooldown = 1f;

    public List<UIIndividualLevelMenu> levelCanvases;

    private int currentLevelsCanvasIndex = 1;
    private int previousLevelCanvasIndex = 1;

    private bool canSwitchPage = true;

    private void Awake()
    {
        levelCanvases = new List<UIIndividualLevelMenu>(); 
    }


    private void OnEnable()
    {
        UpdateStarsCount();

        if(levelCanvases.Count == 0) CreateLevelMenus();

        foreach(var levelCanvas in levelCanvases)
        {
            levelCanvas.UpdateButtons();
        }

        menuScreen.SetActive(true);

        int unlockedLevel = PlayerPrefs.GetInt(PrefsKeys.UnlockedLevelKey, 1);
        currentLevelsCanvasIndex = GetCanvasIndexForLevel(unlockedLevel);

        //Debug.Log($"OnEnable - Unlocked Level: {unlockedLevel}, Canvas Index: {currentLevelsCanvasIndex}");
        UpdateLevelCanvas(currentLevelsCanvasIndex);
    }

    public void StartSpecificLevel(UILevelButton levelButton, int levelIndex)
    {
        if (!levelButton.IsClickable) return;
        menuManager.StartSpecificLevel(loadingScreen, levelIndex);
    }

    public void OnNextPage()
    {
        if (!canSwitchPage) return;

        DeactivatePreviousLevelCanvas(true, false);

        currentLevelsCanvasIndex = Mathf.Min(currentLevelsCanvasIndex + 1, levelCanvases.Count - 1);
        UpdateLevelCanvas(currentLevelsCanvasIndex);

        StartCoroutine(WaitToSwitchPage());
    }


    public void OnBackPage()
    {
        if (!canSwitchPage) return;

        DeactivatePreviousLevelCanvas(false, true);

        currentLevelsCanvasIndex = Mathf.Max(currentLevelsCanvasIndex - 1, 0);
        UpdateLevelCanvas(currentLevelsCanvasIndex);

        StartCoroutine(WaitToSwitchPage());
    }

    public void OnBackToMenu()
    {
        menuManager.CloseScreen(menuScreen);

        for (int i = 0; i < levelCanvases.Count; i++)
        {
            if (!levelCanvases[i].gameObject.activeSelf) continue;
            menuManager.CloseScreen(levelCanvases[i].gameObject);
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

            int startLevel = i == 0 ? i * levelsPerCanvas + 2 : i * levelsPerCanvas + 1;
            int endLevel = Mathf.Min((i + 1) * levelsPerCanvas + 1, totalLevels);

            individualLevelMenu.Setup(startLevel, endLevel, eventSystem, this, i == 0);
        }
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

    private void DeactivatePreviousLevelCanvas(bool shouldDeactivateFirstCanvas, bool shouldDeactivateLastCanvas)
    {
        previousLevelCanvasIndex = currentLevelsCanvasIndex;

        if (previousLevelCanvasIndex == 0 && !shouldDeactivateFirstCanvas) return;
        if (previousLevelCanvasIndex == levelCanvases.Count - 1 && !shouldDeactivateLastCanvas) return;

        StopAllCoroutines();
        canSwitchPage = false;

        menuManager.CloseScreen(levelCanvases[previousLevelCanvasIndex].gameObject);
    }

    private void UpdateStarsCount()
    {
        int maxStars = totalLevels * 3;

        int totalStars = PlayerPrefs.GetInt(PrefsKeys.TotalStarsKey, 0);
        starsCountText.text = $"{totalStars}/{maxStars}";
    }

    /// <summary>
    /// The canvas that will open when the play button is pressed will always be 
    /// the canvas containing the button for the last unlocked level.
    /// </summary>
    private int GetCanvasIndexForLevel(int level)
    {
        int levelsPerCanvas = 9;
        int canvasIndex = ((level) / levelsPerCanvas);

        //Debug.Log($"Level: {level}, Canvas Index: {canvasIndex}");

        return Mathf.Clamp(canvasIndex, 0, levelCanvases.Count - 1);
    }

    private IEnumerator WaitToSwitchPage()
    {
        yield return new WaitForSeconds(switchPageCooldown);

        canSwitchPage = true;
    }
}
