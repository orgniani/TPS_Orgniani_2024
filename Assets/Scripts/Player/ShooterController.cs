using UnityEngine;
using StarterAssets;
using System;

public class ShooterController : WeaponController
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


    private HealthController targetHP;
    private Vector3 hitPoint;

    public event Action onAmmoChange = delegate { };

    public float AmmoAmount => ammoAmount;
    public float MaxAmmoAmount => maxAmmoAmount;
    public bool IsPointingAtEnemy { get; private set; }

    private void Update()
    {
        if (Cursor.lockState != CursorLockMode.Locked) return;
        if (!enabled) return;

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
        if (!starterAssetInputs.aim)
        {
            starterAssetInputs.shoot = false;
            return;
        }

        if (ammoAmount <= 0) return;

        ammoAmount--;
        onAmmoChange?.Invoke();

        gunSmoke.Play();
        //shotSound.Play();

        ShotFeedback shotFeedback = Instantiate(shotPrefab, gunTip.position, Quaternion.identity);

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

    public void ReplenishAmmo()
    {
        ammoAmount = maxAmmoAmount;
        onAmmoChange?.Invoke();
    }
}
