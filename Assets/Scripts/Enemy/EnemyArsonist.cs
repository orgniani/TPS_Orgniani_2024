using System;
using System.Collections;
using UnityEngine;

public class EnemyArsonist : Enemy
{
    [Header("Parameters")]
    [SerializeField] private float maxDistanceToTarget = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip lightOnFireSound;
    
    private bool shouldLightFire = true;

    public event Action onLightFire = delegate { };

    private void OnEnable()
    {
        FlammableObject.onSpawn += LightOnFireTargets;
        FlammableObject.onExtinguished += LightOnFireTargets;
        FlammableObject.onFire += RemoveFromLightOnFire;
        FlammableObject.onDeath += RemoveFromLightOnFire;
    }

    private void OnDisable()
    {
        foreach (Transform point in patrolPoints)
        {
            FlammableObject.onSpawn -= LightOnFireTargets;
            FlammableObject.onExtinguished -= LightOnFireTargets;
            FlammableObject.onFire -= RemoveFromLightOnFire;
            FlammableObject.onDeath -= RemoveFromLightOnFire;
        }
    }

    private void RemoveFromLightOnFire(FlammableObject obj)
    {
        if (obj == null || patrolPoints == null) return;
        patrolPoints.Remove(obj.transform);
    }

    private void LightOnFireTargets(FlammableObject obj)
    {
        if (obj == null || patrolPoints == null) return;
        if (!obj.gameObject.activeSelf) return;

        if (!patrolPoints.Contains(obj.transform))
        {
            patrolPoints.Add(obj.transform);
        }
    }

    private void Update()
    {
        if (!enabled || patrolPoints == null) return;

        if (shouldLightFire) Patrol();
        LightOnFire();
    }

    private void LightOnFire()
    {
        if (!shouldLightFire || patrolPoints == null || patrolPoints.Count == 0) return;

        float distanceToCurrentTarget = Vector3.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position);

        if (distanceToCurrentTarget <= maxDistanceToTarget)
        {
            StartCoroutine(StopAndLight());

            FlammableObject flammableObject = patrolPoints[currentPatrolPointIndex].GetComponent<FlammableObject>();
            flammableObject.HandleGetLitOnFire();

            if (lightOnFireSound) audioSource.PlayOneShot(lightOnFireSound);
        }
    }

    private IEnumerator StopAndLight()
    {
        shouldLightFire = false;

        agent.isStopped = true;
        onLightFire?.Invoke();

        yield return new WaitForSeconds(1f);

        shouldLightFire = true;
        agent.isStopped = false;
    }
}
