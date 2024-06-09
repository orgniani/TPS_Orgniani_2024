using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthController playerHP;

    [Header("Lists")]
    [SerializeField] private List<Enemy> enemies;
    public List<FlammableObject> flammables;

    [Header("Audio")]
    [SerializeField] private AudioSource winGameSoundEffect;
    [SerializeField] private AudioSource loseGameSoundEffect;

    [Header("Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompletedScreen;
    [SerializeField] private float waitForLoseScreen = 3f;

    [Header("Text")]
    [SerializeField] private UITimeCounter timeCounter;
    [SerializeField] private TextMeshProUGUI gameOverText;

    public int flammablesTotal;

    public event Action onNewDeadTree = delegate { };

    private void OnEnable()
    {
        playerHP.onDead += LoseGame;

        Enemy.onSpawn += SpawnedEnemy;
        Enemy.onTrapped += KillCounter;

        FlammableObject.onSpawn += SpawnedFlammable;
        FlammableObject.onDeath += DeadNatureCounter;
    }

    private void OnDisable()
    {
        playerHP.onDead -= LoseGame;

        Enemy.onSpawn -= SpawnedEnemy;
        Enemy.onTrapped -= KillCounter;

        FlammableObject.onSpawn -= SpawnedFlammable;
        FlammableObject.onDeath -= DeadNatureCounter;
    }

    private void KillCounter(Enemy obj)
    {
        enemies.Remove(obj);

        if (enemies.Count == 0)
        {
            StartCoroutine(StopGameAndOpenScreens(levelCompletedScreen, winGameSoundEffect, 0f));
        }
    }

    private void DeadNatureCounter(FlammableObject obj)
    {
        flammables.Remove(obj);
        onNewDeadTree?.Invoke();

        if (flammables.Count == 0)
        {
            StartCoroutine(StopGameAndOpenScreens(gameOverScreen, loseGameSoundEffect, 0f));
            gameOverText.text = "THE FOREST WAS DESTROYED!";
        }
    }

    private void SpawnedEnemy(Enemy obj)
    {
        if (!enemies.Contains(obj))
        {
            enemies.Add(obj);
        }
    }

    private void SpawnedFlammable(FlammableObject obj)
    {
        flammables.Add(obj);
        flammablesTotal++;
    }

    private void LoseGame()
    {
        StartCoroutine(StopGameAndOpenScreens(gameOverScreen, loseGameSoundEffect, waitForLoseScreen));
        gameOverText.text = "YOU DIED!";
    }

    private IEnumerator StopGameAndOpenScreens(GameObject screen, AudioSource soundEffect, float waitForScreen)
    {
        Cursor.lockState = CursorLockMode.None;

        soundEffect.Play();
        timeCounter.StopCounting();

        yield return new WaitForSeconds(waitForScreen);

        screen.SetActive(true);
        enabled = false;
    }
}
