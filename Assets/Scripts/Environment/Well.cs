using System.Collections;
using UnityEngine;

public class Well : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerEnvironmentInteraction playerEnvironmentInteraction;
    [SerializeField] private ParticleSystem waterParticleSystemPrefab;

    [Header("Audio")]
    [SerializeField] private AudioClip waterSound;

    private AudioSource audioSource;

    private bool isSplashing = false;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        playerEnvironmentInteraction.onSplash += HandleSplash;
    }

    private void OnDisable()
    {
        playerEnvironmentInteraction.onSplash -= HandleSplash;
    }

    private void HandleSplash()
    {
        if (isSplashing) return;

        isSplashing = true;

        waterParticleSystemPrefab.Play();
        audioSource.PlayOneShot(waterSound);

        StartCoroutine(ResetSplashState());
    }

    private IEnumerator ResetSplashState()
    {
        yield return new WaitForSeconds(waterSound.length);

        isSplashing = false;
    }
}
