using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class UIEndScreen : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameManager gameManager;
    [SerializeField] private EventSystem eventSystem;

    [Header("Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompletedScreen;
    [SerializeField] private float waitForLoseScreen = 3f;

    [Header("Text")]
    [SerializeField] private UITimeCounter timeCounter;
    [SerializeField] private TextMeshProUGUI gameOverText;

    [Header("Buttons")]
    [SerializeField] private GameObject winScreenMainMenuButton;
    [SerializeField] private GameObject loseScreenMainMenuButton;

    private void OnEnable()
    {
        gameManager.onLevelEnded += HandleOpenEndScreen;
    }

    private void OnDisable()
    {
        gameManager.onLevelEnded -= HandleOpenEndScreen;
    }

    private void HandleOpenEndScreen(GameManager.GameEndedReason gameEndedReason)
    {
        switch(gameEndedReason)
        {
            case GameManager.GameEndedReason.WIN:
                StartCoroutine(StopGameAndOpenScreens(levelCompletedScreen, 0f, winScreenMainMenuButton));
                break;
            case GameManager.GameEndedReason.FOREST_KILLED:
                StartCoroutine(StopGameAndOpenScreens(gameOverScreen, 0f, loseScreenMainMenuButton));
                gameOverText.text = "THE FOREST WAS DESTROYED!";
                break;
            case GameManager.GameEndedReason.PLAYER_KILLED:
                StartCoroutine(StopGameAndOpenScreens(gameOverScreen, waitForLoseScreen, loseScreenMainMenuButton));
                gameOverText.text = "YOU DIED!";
                break;
        }
    }

    private IEnumerator StopGameAndOpenScreens(GameObject screen, float waitForScreen, GameObject button)
    {
        Cursor.lockState = CursorLockMode.None;

        timeCounter.StopCounting();

        yield return new WaitForSeconds(waitForScreen);

        eventSystem.SetSelectedGameObject(button);

        screen.SetActive(true);
        enabled = false;
    }
}
