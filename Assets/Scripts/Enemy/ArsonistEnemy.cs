using System;
using System.Collections;
using UnityEngine;

public class ArsonistEnemy : Enemy
{
    [Header("Parameters")]
    [SerializeField] private float maxDistanceToTarget = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip lightOnFireSound;

    private bool shouldLightFire = true;

    public event Action onLightFire = delegate { };

    protected override void OnEnable()
    {
        base.OnEnable();

        FlammableObject.onSpawn += LightOnFireTargets;
        FlammableObject.onExtinguished += LightOnFireTargets;
        FlammableObject.onFire += RemoveFromLightOnFire;
        FlammableObject.onDeath += RemoveFromLightOnFire;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

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
        patrolPoints.Remove(obj.transform);
    }

    private void LightOnFireTargets(FlammableObject obj)
    {
        if (obj == null || !obj.gameObject.activeSelf) return;
        if (patrolPoints.Contains(obj.transform)) return;

        patrolPoints.Add(obj.transform);
    }

    private void Update()
    {
        if (!isAwake) return;

        Patrol();
    }

    protected override void Patrol()
    {
        if (!shouldLightFire) return;

        if (patrolPoints.Count == 0)
        {
            agent.isStopped = true;
            return;
        }

        Vector3 nextPoint = patrolPoints[currentPatrolPointIndex].transform.position;
        float distanceToCurrentTarget = Vector3.Distance(transform.position, patrolPoints[currentPatrolPointIndex].position);

        if (distanceToCurrentTarget <= maxDistanceToTarget)
        {
            StartCoroutine(StopAndLight());

            FlammableObject flammableObject = patrolPoints[currentPatrolPointIndex].GetComponent<FlammableObject>();
            flammableObject.HandleGetLitOnFire();

            if (lightOnFireSound) audioSource.PlayOneShot(lightOnFireSound);

            SetNextPatrolPoint(distanceToCurrentTarget);
        }
        
        else
            agent.SetDestination(nextPoint);
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

    private void SetNextPatrolPoint(float distanceToCurrentTarget)
    {
        if (distanceToCurrentTarget < maxDistanceToTarget)
        {
            currentPatrolPointIndex++;

            if (currentPatrolPointIndex >= patrolPoints.Count)
            {
                currentPatrolPointIndex = 0;
            }
        }
    }
}
