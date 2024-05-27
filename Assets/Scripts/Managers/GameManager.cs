using StarterAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private HealthController playerHP;
    [SerializeField] private LayerMask playerLayer;

    [Header("Audio")]
    [SerializeField] private AudioSource winGameSoundEffect;
    [SerializeField] private AudioSource loseGameSoundEffect;

    [Header("Screens")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject levelCompletedScreen;

    [SerializeField] private float waitForLoseScreen = 3f;

    private void OnEnable()
    {
        playerHP.onDead += LoseGame;
    }

    private void OnDisable()
    {
        playerHP.onDead -= LoseGame;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerLayer == (playerLayer | (1 << other.gameObject.layer)))
        {
            StartCoroutine(StopGameAndOpenScreens(levelCompletedScreen, winGameSoundEffect, 0f));
            playerHP.gameObject.SetActive(false);
        }
    }

    private void LoseGame()
    {
        StartCoroutine(StopGameAndOpenScreens(gameOverScreen, loseGameSoundEffect, waitForLoseScreen));
    }

    private IEnumerator StopGameAndOpenScreens(GameObject screen, AudioSource soundEffect, float waitForScreen)
    {
        Cursor.lockState = CursorLockMode.None;

        if(soundEffect) soundEffect.Play();

        yield return new WaitForSeconds(waitForScreen);

        screen.SetActive(true);
        enabled = false;
    }
}
