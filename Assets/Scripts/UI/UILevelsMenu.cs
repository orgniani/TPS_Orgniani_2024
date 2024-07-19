using System.Collections;
using Unity.Burst.CompilerServices;
using UnityEngine;

public class UILevelsMenu : MonoBehaviour
{
    [Header("Canvases")]
    [SerializeField] private GameObject[] levelCanvases;
    [SerializeField] private GameObject loadingScreen;

    [Header("Managers")]
    [SerializeField] private MenuManager menuManager;

    [Header("Parameters")]
    [SerializeField] private float switchPageCooldown = 1f;

    private int currentLevelsCanvasIndex = 1;
    private int previousLevelCanvasIndex = 1;

    private bool canSwitchPage = true;

    private void OnEnable()
    {
        UpdateLevelCanvas(0);
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
        canSwitchPage = false;

        DeactivatePreviousLevelCanvas(true, false);

        currentLevelsCanvasIndex = Mathf.Min(currentLevelsCanvasIndex + 1, levelCanvases.Length - 1);
        UpdateLevelCanvas(currentLevelsCanvasIndex);

        StartCoroutine(WaitToSwitchPage());
    }


    public void OnBackPage()
    {
        if (!canSwitchPage) return;
        canSwitchPage = false;

        DeactivatePreviousLevelCanvas(false, true);

        currentLevelsCanvasIndex = Mathf.Max(currentLevelsCanvasIndex - 1, 1);
        UpdateLevelCanvas(currentLevelsCanvasIndex);

        StartCoroutine(WaitToSwitchPage());
    }

    public void OnBackToMenu()
    {
        for (int i = 0; i < levelCanvases.Length; i++)
        {
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

        menuManager.CloseScreen(levelCanvases[previousLevelCanvasIndex]);
    }

    private IEnumerator WaitToSwitchPage()
    {
        yield return new WaitForSeconds(switchPageCooldown);

        canSwitchPage = true;
    }
}
