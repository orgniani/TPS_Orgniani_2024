using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIIndividualLevelMenu : MonoBehaviour
{
    [SerializeField] private UILevelButton[] buttons;
    [SerializeField] private UILevelsMenu levelsMenu;

    [SerializeField] private EventSystem eventSystem;

    private void Awake()
    {
        int unlockedLevel = PlayerPrefs.GetInt(PrefsKeys.UnlockedLevelKey, 2);

        for (int i = 0; i < buttons.Length; i++)
        {
            bool isUnlocked = buttons[i].LevelIndex <= unlockedLevel;
            buttons[i].IsClickable = isUnlocked;

            buttons[i].LockImage.SetActive(!isUnlocked);
            buttons[i].LevelText.SetActive(isUnlocked);

            int buttonIndex = i;
            int levelIndex = buttons[i].LevelIndex;

            buttons[i].GetComponent<Button>().onClick.RemoveAllListeners();
            buttons[i].GetComponent<Button>().onClick.AddListener(() => OnButtonClicked(buttonIndex, levelIndex));

            if(buttons[i].IsClickable) eventSystem.SetSelectedGameObject(buttons[i].gameObject);
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