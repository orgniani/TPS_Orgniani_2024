using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected HealthController targetHP;
    [SerializeField] protected List<Transform> patrolPoints;

    [Header("Parameters")]
    [SerializeField] private float passedOutDuration = 10f;
    [SerializeField] protected float proximityRadius = 5f;

    [Header("Audio")]
    [SerializeField] private AudioClip deathSound;
    [SerializeField] private AudioClip wakeUpSound;

    [Header("Type")]
    [SerializeField] private EnemyType enemyType;
    [SerializeField] protected LayerMask targetLayer;

    protected AudioSource audioSource;
    protected NavMeshAgent agent;
    protected HealthController HP;

    private EnemyArsonist arsonist;
    private EnemyPatrol patrol;

    private bool isAwake = false;
    protected int currentPatrolPointIndex = 0;

    public static event Action<Enemy> onSpawn;
    public static event Action<Enemy> onTrapped;
    public static event Action<Enemy> onKnockedOut;
    public static event Action<Enemy> onWakeUp;

    public event Action onWakeUpAnimation;

    private Coroutine wakeUpCoroutine;
    private Coroutine enableCoroutine;

    private enum EnemyType { PATROL = 0, ARSONIST, BOMB }

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
        HP = GetComponent<HealthController>();

        if(enemyType == EnemyType.PATROL)
            patrol = GetComponent<EnemyPatrol>();

        if (enemyType == EnemyType.ARSONIST)
            arsonist = GetComponent<EnemyArsonist>();

        if (patrolPoints == null)
            patrolPoints = new List<Transform>();
    }

    private void OnEnable()
    {
        HP.onHPChange += HandleKnockedOut;
        targetHP.onDead += HandleStopMoving;
    }

    private void OnDisable()
    {
        HP.onHPChange -= HandleKnockedOut;
        targetHP.onDead -= HandleStopMoving;
    }

    private void Start()
    {
        if(enemyType != EnemyType.BOMB) onSpawn?.Invoke(this);
    }

    private void HandleKnockedOut()
    {
        if (wakeUpCoroutine != null)
            StopCoroutine(wakeUpCoroutine);
        if (enableCoroutine != null)
            StopCoroutine(enableCoroutine);

        if (enemyType != EnemyType.BOMB) onKnockedOut?.Invoke(this);

        isAwake = false;

        if (deathSound) audioSource.PlayOneShot(deathSound);

        EnableAndDisableEnemyType(false);

        agent.isStopped = true;

        wakeUpCoroutine = StartCoroutine(WaitToWakeBackUp());
    }

    private IEnumerator WaitToWakeBackUp()
    {
        yield return new WaitForSeconds(passedOutDuration);

        onWakeUp?.Invoke(this);
        onWakeUpAnimation?.Invoke();

        HP.SetToMaxHealth();

        if (wakeUpSound) audioSource.PlayOneShot(wakeUpSound);
        isAwake = true;

        enableCoroutine = StartCoroutine(WaitToEnable());
    }

    private IEnumerator WaitToEnable()
    {
        while (!isAwake)
        {
            yield return null;
        }

        yield return new WaitForSeconds(1);

        agent.isStopped = false;
        agent.stoppingDistance = 2;

        EnableAndDisableEnemyType(true);
    }

    public void HandleGetTrapped()
    {
        EnableAndDisableEnemyType(false);
        onTrapped?.Invoke(this);
        Destroy(gameObject);
    }

    private void HandleStopMoving()
    {
        EnableAndDisableEnemyType(false);
        enabled = false;
    }

    private void EnableAndDisableEnemyType(bool enabled)
    {
        this.enabled = enabled;
    }

    protected void Patrol()
    {
        if (patrolPoints.Count == 0)
        {
            agent.isStopped = true;
            return;
        }

        agent.isStopped = false;

        Vector3 nextPoint = patrolPoints[currentPatrolPointIndex].transform.position;

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            SetNextPatrolPoint();
        }

        agent.SetDestination(nextPoint);
    }

    private void SetNextPatrolPoint()
    {
        if (patrolPoints == null || patrolPoints.Count == 0) return;

        if (enemyType != EnemyType.ARSONIST) currentPatrolPointIndex++;

        if (currentPatrolPointIndex >= patrolPoints.Count)
        {
            currentPatrolPointIndex = 0;
        }
    }

    protected bool PlayerIsTooClose()
    {
        return Physics.CheckSphere(transform.position, proximityRadius, targetLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, proximityRadius);
    }
}
