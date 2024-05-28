using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using System;

public class ShooterController : MonoBehaviour
{
    [Header("Aiming")]
    [Header("References")]
    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] private Transform debugTransform;

    [Header("Parameters")]
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
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

    private StarterAssetsInputs starterAssetInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    private Vector3 mouseWorldPosition;
    private float aimRigWeight;

    private HealthController targetHP;
    private Vector3 hitPoint;

    public event Action onAmmoChange = delegate { };

    public float AmmoAmount => ammoAmount;
    public float MaxAmmoAmount => maxAmmoAmount;
    public bool IsPointingAtEnemy { get; private set; }

    private void Awake()
    {
        starterAssetInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (!enabled) return;

        CheckIfPointingAtEnemy();
        HandlePlayerAiming();

        if(starterAssetInputs.shoot)
        {
            Shoot();
        }
    }

    private void HandlePlayerAiming()
    {
        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);

        mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if (starterAssetInputs.aim)
        {
            aimVirtualCamera.gameObject.SetActive(true);
            thirdPersonController.SetSensitivity(aimSensitivity);
            thirdPersonController.SetRotateOnMove(false);
            thirdPersonController.SetSprintOnAim(false);
            thirdPersonController.SetStrafeOnAim(true);

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
            aimRigWeight = 1f;

            Vector3 worldAimTarget = mouseWorldPosition;
            worldAimTarget.y = transform.position.y;
            Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

            transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
        }

        else
        {
            aimVirtualCamera.gameObject.SetActive(false);
            thirdPersonController.SetSensitivity(normalSensitivity);
            thirdPersonController.SetRotateOnMove(true);
            thirdPersonController.SetStrafeOnAim(false);

            animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
            aimRigWeight = 0f;
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

    public void Shoot()
    {
        if (!starterAssetInputs.aim)
        {
            starterAssetInputs.shoot = false;
            return;
        }

        if (Cursor.lockState != CursorLockMode.Locked) return;

        if (ammoAmount <= 0) return;

        ammoAmount--;
        onAmmoChange?.Invoke();

        gunSmoke.Play();
        //shotSound.Play();

        ShotFeedback shotFeedback = Instantiate(shotPrefab, gunTip.position, Quaternion.identity);

        Vector3 cameraForward = Camera.main.transform.forward;

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
