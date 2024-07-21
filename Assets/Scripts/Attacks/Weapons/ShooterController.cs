using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.Pool;
using System.Collections;

public class ShooterController : AttackController
{
    [Header("Aiming")]
    [Header("References")]
    [SerializeField] private LayerMask enemies;

    [Header("Shooting")]
    [Header("References")]
    [SerializeField] private ShotFeedback shotPrefab;
    [SerializeField] private Transform gunTip;
    [SerializeField] private ParticleSystem gunSmoke;

    [Header("Parameters")]
    [SerializeField] private float gunDamage = 10f;

    [Header("Ammo")]
    [Header("Parameters")]
    [SerializeField] private float ammoAmount = 2f;
    [SerializeField] private float maxAmmoAmount = 5f;

    [Header("Audio")]
    [SerializeField] private AudioSource shotSound;

    private HealthController targetHP;
    private Vector3 hitPoint;

    private List<ShotFeedback> activeShotFeedback = new List<ShotFeedback>();
    private List<ShotFeedback> shotFeedbackPool = new List<ShotFeedback>();

    public event Action onAmmoChange = delegate { };

    public float AmmoAmount => ammoAmount;
    public float MaxAmmoAmount => maxAmmoAmount;
    public bool IsPointingAtEnemy { get; private set; }

    private void Update()
    {
        if (activeWeapon != ActiveAttackSetter.ActiveWeapon.GUN) return;

        if (hasAnimator)
        {
            animator.SetTrigger(animIDGun);
        }

        HandlePlayerAiming();
        CheckIfPointingAtEnemy();

        if (starterAssetInputs.shoot)
        {
            Shoot();
        }
    }


    private void CheckIfPointingAtEnemy()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;

        RaycastHit hit;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 sourcePos = gunTip.position;

        if (Physics.Raycast(sourcePos, cameraForward, out hit, Mathf.Infinity, enemies))
        {
            targetHP = hit.transform.GetComponentInParent<HealthController>();
            hitPoint = hit.point;

            IsPointingAtEnemy = true;
        }
        else
        {
            IsPointingAtEnemy = false;
        }
    }

    public override void Shoot()
    {
        if (ammoAmount <= 0) return;

        ammoAmount--;
        onAmmoChange?.Invoke();

        gunSmoke.Play();
        shotSound.Play();

        ShotFeedback shotFeedback = GetShotFeedbackFromPool();

        if (IsPointingAtEnemy)
        {
            targetHP.ReceiveDamage(gunDamage, hitPoint);
            shotFeedback.ShowShotDirection(hitPoint);
        }

        else
        {
            shotFeedback.ShowShotDirection(debugTransform.position);
        }

        starterAssetInputs.shoot = false;
    }

    private ShotFeedback GetShotFeedbackFromPool()
    {
        ShotFeedback shotFeedback;

        if (shotFeedbackPool.Count == 0)
        {
            shotFeedback = Instantiate(shotPrefab, gunTip.position, Quaternion.identity);
            activeShotFeedback.Add(shotFeedback);

            StartCoroutine(WaitToDeactivateFeedback(shotFeedback));
            return shotFeedback;
        }

        else
        {
            ShotFeedback reusedShotFeedback = shotFeedbackPool[0];

            reusedShotFeedback.transform.position = gunTip.position;
            reusedShotFeedback.transform.rotation = Quaternion.identity;

            reusedShotFeedback.gameObject.SetActive(true);

            activeShotFeedback.Add(reusedShotFeedback);
            shotFeedbackPool.Remove(reusedShotFeedback);

            StartCoroutine(WaitToDeactivateFeedback(reusedShotFeedback));
            return reusedShotFeedback;
        }
    }

    private IEnumerator WaitToDeactivateFeedback(ShotFeedback shotFeedback)
    {
        yield return new WaitForSeconds(0.1f);

        activeShotFeedback.Remove(shotFeedback);
        shotFeedbackPool.Add(shotFeedback);

        shotFeedback.gameObject.SetActive(false);
    }

    public void ReplenishAmmo()
    {
        ammoAmount = maxAmmoAmount;
        onAmmoChange?.Invoke();
    }
}
