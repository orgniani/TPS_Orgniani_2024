using StarterAssets;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class HandController : AttackController
{
    [Header("References")]
    [SerializeField] private ThirdPersonController TPSController;
    [SerializeField] private AudioSource trapGoblinSound;

    [Header("Parameters")]
    [SerializeField] private float proximityRadius = 5f;
    [SerializeField] private float dragSpeed = 2f;

    [Header("Enemies")]
    [SerializeField] private List<Transform> knockedOutEnemies;

    private NavMeshAgent enemyAgent;

    private float moveSpeed = 2f;

    private Transform currentlyDraggedEnemy;
    private bool isDraggingEnemy = false;

    public event Action onClick = delegate { };

    public bool IsDraggingEnemy => isDraggingEnemy;

    public bool CanDrag { set; get; }

    public bool IsAtTheDoor { set; get; }


    private void OnEnable()
    {
        moveSpeed = TPSController.MoveSpeed;

        Enemy.onKnockedOut += AddToEnemiesList;
        Enemy.onWakeUp += RemoveFromEnemiesList;
        Enemy.onTrapped += RemoveFromEnemiesList;
    }

    private void OnDisable()
    {
        Enemy.onKnockedOut -= AddToEnemiesList;
        Enemy.onWakeUp -= RemoveFromEnemiesList;
        Enemy.onTrapped -= RemoveFromEnemiesList;
    }

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (!CanDrag)
        {
            StopDragging();
            return;
        }

        HandlePlayerAiming();

        if (starterAssetInputs.aim)
        {
            if (hasAnimator)
            {
                animator.SetTrigger(animIDDrag);
            }

            DragEnemy();
        }

        else
        {
            StopDragging();
        }

        if (starterAssetInputs.shoot)
        {
            onClick?.Invoke();
            HandleTrapGoblin();
        }
    }

    private void DragEnemy()
    {
        Transform nearestEnemy = FindNearestKnockedOutEnemy();

        if (nearestEnemy != null)
        {
            float distanceToNearestEnemy = Vector3.Distance(transform.position, nearestEnemy.position);

            if (distanceToNearestEnemy <= proximityRadius)
            {
                enemyAgent = nearestEnemy.GetComponent<NavMeshAgent>();

                if (enemyAgent != null)
                {
                    enemyAgent.isStopped = false;
                    enemyAgent.stoppingDistance = 1;
                    enemyAgent.SetDestination(transform.position);

                    TPSController.MoveSpeed = dragSpeed;

                    isDraggingEnemy = true;
                    currentlyDraggedEnemy = nearestEnemy;

                    RotateTowardsTarget();
                }
            }
        }

        else
        {
            StopDragging();
        }
    }

    private void RotateTowardsTarget()
    {
        if (currentlyDraggedEnemy != null)
        {
            Vector3 aimDirection = (currentlyDraggedEnemy.position - transform.position).normalized;
            aimDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(aimDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 20f);
        }
    }

    public void StopDragging()
    {
        isDraggingEnemy = false;
        TPSController.MoveSpeed = moveSpeed;
        thirdPersonController.SetRotateOnMove(true);
        thirdPersonController.SetStrafeOnAim(false);

        if (currentlyDraggedEnemy != null) currentlyDraggedEnemy = null;

        if (enemyAgent == null) return;
        enemyAgent.isStopped = true;

    }

    private Transform FindNearestKnockedOutEnemy()
    {
        Transform nearestEnemy = null;
        float minDistance = Mathf.Infinity;

        foreach (Transform enemyTransform in knockedOutEnemies)
        {
            if (enemyTransform == null) continue;

            float distanceToEnemy = Vector3.Distance(transform.position, enemyTransform.position);

            if (distanceToEnemy < minDistance)
            {
                nearestEnemy = enemyTransform;
                minDistance = distanceToEnemy;
            }
        }

        return nearestEnemy;
    }

    private void AddToEnemiesList(Enemy obj)
    {
        if (!knockedOutEnemies.Contains(obj.transform))
        {
            knockedOutEnemies.Add(obj.transform);
        }
    }

    private void RemoveFromEnemiesList(Enemy obj)
    {
        knockedOutEnemies.Remove(obj.transform);
    }

    private void HandleTrapGoblin()
    {
        if (!IsAtTheDoor || currentlyDraggedEnemy == null) return;

        Enemy enemyScript = currentlyDraggedEnemy.GetComponent<Enemy>();

        if (enemyScript != null)
        {
            trapGoblinSound.Play();
            enemyScript.HandleGetTrapped();
        }

        enemyAgent = null;
        isDraggingEnemy = false;

        starterAssetInputs.shoot = false;
    }
}
