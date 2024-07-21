using System;
using UnityEngine;

public class FireDeath : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private LayerMask fireFoamLayer;
    [SerializeField] private ParticleSystem fireInstance;

    private AudioSource audioSource;

    public event Action onDeath = delegate { };

    private void OnEnable()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.Play();
    }

    private void OnParticleCollision(GameObject other)
    {
        if ((fireFoamLayer.value & (1 << other.layer)) != 0)
        {
            HandleFireDeath();
        }
    }

    public void HandleFireDeath()
    {
        fireInstance.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }

    private void OnDisable()
    {
        onDeath?.Invoke();
    }
}
