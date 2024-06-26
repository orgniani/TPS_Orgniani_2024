using System;
using System.Collections;
using UnityEngine;

public class BombEnemy : Enemy
{
    [Header("References")]
    [SerializeField] private ParticleSystem explosionPrefab;

    [Header("Parameters")]
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionDamage = 10;
    [SerializeField] private float countdownDuration = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource explotionSound;
    [SerializeField] private AudioSource countingDownSound;

    private bool isCountingDown = false;

    public event Action onCloseToPlayer = delegate { };
    public static event Action onExplode;

    protected override void OnEnable()
    {
        HP.onDead += HandleExplosion;
    }

    protected override void OnDisable()
    {
        HP.onDead -= HandleExplosion;
    }

    private void Update()
    {
        ChaseTarget();
        CheckIfPlayerClose();
    }

    private void HandleExplosion()
    {
        onExplode?.Invoke();
        explotionSound.Play();
        Instantiate(explosionPrefab, transform.position, transform.rotation);

        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            HealthController targetHP = hit.GetComponent<HealthController>();
            FlammableObject flammableObject = hit.GetComponent<FlammableObject>();

            if (rb != null)
            {
                rb.isKinematic = false;

                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            if (flammableObject != null) flammableObject.HandleGetLitOnFire();

            if (targetHP != null && hit.CompareTag("Player")) targetHP.ReceiveDamage(explosionDamage, hit.transform.position);
        }

        HandleGetTrapped();
    }

    private void CheckIfPlayerClose()
    {
        if (PlayerIsTooClose() && !isCountingDown)
        {
            countingDownSound.Play();
            onCloseToPlayer?.Invoke();

            StartCoroutine(InitiateCountdown());
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