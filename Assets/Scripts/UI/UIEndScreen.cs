using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIEndScreen : MonoBehaviour
{
    [SerializeField] private GameManager gameManager;

    [Header("Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompletedScreen;
    [SerializeField] private float waitForLoseScreen = 3f;

    [Header("Text")]
    [SerializeField] private UITimeCounter timeCounter;
    [SerializeField] private TextMeshProUGUI gameOverText;

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
                StartCoroutine(StopGameAndOpenScreens(levelCompletedScreen, 0f));
                break;
            case GameManager.GameEndedReason.FOREST_KILLED:
                StartCoroutine(StopGameAndOpenScreens(gameOverScreen, 0f));
                gameOverText.text = "THE FOREST WAS DESTROYED!";
                break;
            case GameManager.GameEndedReason.PLAYER_KILLED:
                StartCoroutine(StopGameAndOpenScreens(gameOverScreen, waitForLoseScreen));
                gameOverText.text = "YOU DIED!";
                break;
        }
    }

    private IEnumerator StopGameAndOpenScreens(GameObject screen, float waitForScreen)
    {
        Cursor.lockState = CursorLockMode.None;

        timeCounter.StopCounting();

        yield return new WaitForSeconds(waitForScreen);

        screen.SetActive(true);
        enabled = false;
    }
}
