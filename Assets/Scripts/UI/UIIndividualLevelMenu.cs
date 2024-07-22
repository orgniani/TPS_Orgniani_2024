using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIIndividualLevelMenu : MonoBehaviour
{
    [SerializeField] private UILevelButton[] buttons;
    
    private EventSystem eventSystem;
    private UILevelsMenu levelsMenu;

    public void Setup(int startLevel, int endLevel, EventSystem getEventSystem, UILevelsMenu menu, bool isFirstCanvas)
    {
        levelsMenu = menu;
        eventSystem = getEventSystem;

        for (int i = 0; i < buttons.Length; i++)
        {
            if (i <= endLevel - startLevel + 1)
            {
                int buttonLevelIndex = startLevel + i;
                int displayLevelNumber = isFirstCanvas ? i + 1 : buttonLevelIndex;

                buttons[i].gameObject.SetActive(true);
                buttons[i].Setup(displayLevelNumber, buttonLevelIndex);

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
        int unlockedLevel = PlayerPrefs.GetInt(PrefsKeys.UnlockedLevelKey, 2);

        for (int i = 0; i < buttons.Length; i++)
        {
            bool isUnlocked = buttons[i].LevelIndex <= unlockedLevel;
            buttons[i].IsClickable = isUnlocked;

            buttons[i].LockImage.SetActive(!isUnlocked);
            buttons[i].LevelText.SetActive(isUnlocked);

            if (buttons[i].IsClickable) eventSystem.SetSelectedGameObject(buttons[i].gameObject);
        }
    }      
    private void OnButtonClicked(int buttonIndex, int levelIndex)
    {
        //Debug.Log($"Button: {buttonIndex} clicked for level index: {levelIndex}");

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