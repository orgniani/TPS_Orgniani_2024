using UnityEngine;

public class LevelManager : MonoBehaviour
{
    [Header("Scene indexes")]
    [SerializeField] private int currentLevelBuildIndex = 1;
    [SerializeField] private int nextLevelBuildIndex = 2;
    [SerializeField] private int mainMenuBuildIndex = 0;

    [Header("Loading")]
    [SerializeField] private int fakeLoadingTime = 2;

    private void Awake()
    {
        if(Time.timeScale == 0)
        {
            Time.timeScale = 1;
        }

        Cursor.lockState = CursorLockMode.None;
    }

    public void StartSpecificLevel(int levelIndex)
    {
        LoadAndOpen(levelIndex);
    }

    public void StartLevel()
    {
        LoadAndOpen(currentLevelBuildIndex);
    }

    public void StartNextLevel()
    {
        LoadAndOpen(nextLevelBuildIndex);
    }
    public void BackToMenu()
    {
        LoadAndOpen(mainMenuBuildIndex);
    }

    private void LoadAndOpen(int sceneBuildIndex)
    {
        LoaderManager.Get().LoadScene(sceneBuildIndex, fakeLoadingTime);
    }
}
