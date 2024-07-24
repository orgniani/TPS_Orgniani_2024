using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIIndividualLevelMenu : MonoBehaviour
{
    [SerializeField] private UILevelButton[] buttons;
    
    private EventSystem eventSystem;
    private UILevelsMenu levelsMenu;

    public void Setup(int startLevel, int endLevel, EventSystem eventSystem, UILevelsMenu levelsMenu)
    {
        this.levelsMenu = levelsMenu;
        this.eventSystem = eventSystem;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i <= endLevel - startLevel + 1)
            {
                int buttonLevelIndex = startLevel + i;
                int displayLevelNumber = buttonLevelIndex - 1;

                buttons[i].gameObject.SetActive(true);
                buttons[i].Setup(displayLevelNumber, buttonLevelIndex);

                buttons[i].IsAvailable = displayLevelNumber <= levelsMenu.CurrentAvailableLevels;

                int buttonIndex = i;
                buttons[i].GetComponent<Button>().onClick.RemoveAllListeners();
                buttons[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClicked(buttonIndex, buttonLevelIndex));
            }

            else
            {
                buttons[i].gameObject.SetActive(false);
            }
        }

        UpdateButtons();
    }

    public void UpdateButtons()
    {
        int unlockedLevel = PlayerPrefs.GetInt(PrefsKeys.UnlockedLevelKey);

        foreach (var button in buttons)
        {
            bool isUnlocked = button.LevelIndex <= unlockedLevel;
            button.IsClickable = isUnlocked;

            button.LockImage.SetActive(!isUnlocked);
            button.LevelText.SetActive(isUnlocked);

            if (button.IsClickable) eventSystem.SetSelectedGameObject(button.gameObject);
        }
    }

    private void OnButtonClicked(int buttonIndex, int levelIndex)
    {
        if (levelIndex >= 0 && levelsMenu != null)
        {
            levelsMenu.StartSpecificLevel(buttons[buttonIndex], levelIndex);
        }

        else
        {
            Debug.LogError($"Invalid level index: {levelIndex} or levelsMenu is null.");
        }
    }
}