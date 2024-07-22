using System.Collections;
using UnityEngine;
using TMPro;

public class UILevelsMenu : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject[] levelCanvases;
    [SerializeField] private GameObject loadingScreen;

    [Header("References")]
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private TMP_Text starsCountText;

    [Header("Parameters")]
    [SerializeField] private float switchPageCooldown = 1f;

    private int currentLevelsCanvasIndex = 1;
    private int previousLevelCanvasIndex = 1;

    private int maxStars = 9;

    private bool canSwitchPage = true;

    private void OnEnable()
    {
        UpdateStarsCount();

        UpdateLevelCanvas(0);

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

        currentLevelsCanvasIndex = Mathf.Min(currentLevelsCanvasIndex + 1, levelCanvases.Length - 1);
        UpdateLevelCanvas(currentLevelsCanvasIndex);

        StartCoroutine(WaitToSwitchPage());
    }


    public void OnBackPage()
    {
        if (!canSwitchPage) return;

        DeactivatePreviousLevelCanvas(false, true);

        currentLevelsCanvasIndex = Mathf.Max(currentLevelsCanvasIndex - 1, 1);
        UpdateLevelCanvas(currentLevelsCanvasIndex);

        StartCoroutine(WaitToSwitchPage());
    }

    public void OnBackToMenu()
    {
        for (int i = 0; i < levelCanvases.Length; i++)
        {
            if (!levelCanvases[i].activeSelf) continue;
            menuManager.CloseScreen(levelCanvases[i]);
        }

        menuManager.CloseScreen(gameObject);
    }

    private void UpdateLevelCanvas(int index)
    {
        levelCanvases[index].SetActive(true);
    }

    private void DeactivatePreviousLevelCanvas(bool shouldDeactivateFirstCanvas, bool shouldDeactivateLastCanvas)
    {
        previousLevelCanvasIndex = currentLevelsCanvasIndex;

        if (previousLevelCanvasIndex == 1 && !shouldDeactivateFirstCanvas) return;
        if (previousLevelCanvasIndex == levelCanvases.Length - 1 && !shouldDeactivateLastCanvas) return;

        StopAllCoroutines();
        canSwitchPage = false;

        menuManager.CloseScreen(levelCanvases[previousLevelCanvasIndex]);
    }

    private void UpdateStarsCount()
    {
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
        int canvasIndex = ((level - 1) / levelsPerCanvas) + 1;

        //Debug.Log($"Level: {level}, Canvas Index: {canvasIndex}");

        return Mathf.Clamp(canvasIndex, 0, levelCanvases.Length - 1);
    }

    private IEnumerator WaitToSwitchPage()
    {
        yield return new WaitForSeconds(switchPageCooldown);

        canSwitchPage = true;
    }
}
