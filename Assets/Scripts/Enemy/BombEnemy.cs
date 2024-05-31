using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class BombEnemy : Enemy
{
    [Header("References")]
    [SerializeField] private ParticleSystem explosionPrefab;

    [Header("Parameters")]
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float explosionDamage = 10;
    [SerializeField] private float countdownDuration = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource countingDownSound;
    [SerializeField] private AudioSource explotionSound;

    private bool isCountingDown = false;

    public event Action onCloseToPlayer = delegate { };

    private void OnEnable()
    {
        HP.onDead += HandleExplosion;
    }

    private void OnDisable()
    {
        HP.onDead -= HandleExplosion;
    }

    private void Update()
    {
        agent.SetDestination(targetHP.gameObject.transform.position);

        if(PlayerIsTooClose() && !isCountingDown)
        {
            countingDownSound.Play();
            onCloseToPlayer?.Invoke();

            StartCoroutine(InitiateCountdown());
        }
    }

    private void HandleExplosion()
    {
        explotionSound.Play();

        Instantiate(explosionPrefab, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, proximityRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            HealthController targetsHP = hit.GetComponent<HealthController>();
            FlammableObject flammableObject = hit.GetComponent<FlammableObject>();

            if (rb != null)
            {
                rb.isKinematic = false;

                rb.AddExplosionForce(explosionForce, transform.position, proximityRadius);
            }

            if(flammableObject != null)
            {
                flammableObject.HandleGetLitOnFire();
            }

            if (targetsHP != null && hit.CompareTag("Player")) targetsHP.ReceiveDamage(explosionDamage, hit.transform.position);
        }
    }

    private IEnumerator InitiateCountdown()
    {
        isCountingDown = true;
        agent.isStopped = true;

        float timer = countdownDuration;

        while (timer > 0f)
        {
            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        countingDownSound.Stop();
        HP.ReceiveDamage(HP.Health, Vector3.zero);
    }
}