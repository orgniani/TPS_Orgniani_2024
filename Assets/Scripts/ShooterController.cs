using UnityEngine;
using Cinemachine;
using StarterAssets;
using UnityEngine.Animations.Rigging;

public class ShooterController : MonoBehaviour
{
    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;

    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();
    [SerializeField] private Transform debugTransform;

    [SerializeField] private ShotFeedback shotPrefab;

    [SerializeField] private float gunDamage = 10f;
    [SerializeField] private float gunRange = 10f;
    [SerializeField] private LayerMask enemies;


    [SerializeField] private Transform gunTip;
    [SerializeField] private Transform midOfScreen;

    private StarterAssetsInputs starterAssetInputs;
    private ThirdPersonController thirdPersonController;
    private Animator animator;

    private Vector3 mouseWorldPosition;

    private float aimRigWeight;

    public bool IsPointingAtEnemy { get; private set; }

    private void Awake()
    {
        starterAssetInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateGunSightColor();
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

    private void UpdateGunSightColor()
    {
        RaycastHit hit;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 sourcePos = gunTip.position;

        if (Physics.Raycast(sourcePos, cameraForward, out hit, Mathf.Infinity, enemies))
        {
            IsPointingAtEnemy = true;
        }
        else
        {
            IsPointingAtEnemy = false;
        }
    }

    public void Shoot()
    {
        if (!starterAssetInputs.aim) return;
        //gunSmoke.Play();
        //shotSound.Play();

        Vector3 aimDirection = (mouseWorldPosition - gunTip.position).normalized;
        ShotFeedback shotFeedback = Instantiate(shotPrefab, gunTip.position, Quaternion.LookRotation(aimDirection, Vector3.up));

        Vector3 endPosition = gunTip.position + mouseWorldPosition * gunRange;

        if (IsPointingAtEnemy)
        {
            //targetHP.ReceiveDamage(gunDamage, hitPoint);
            //shotFeedback.ShowShotDirection(hitPoint);

            shotFeedback.ShowShotDirection(endPosition);
        }

        else
        {
            shotFeedback.ShowShotDirection(endPosition);
        }

        starterAssetInputs.shoot = false;
    }
}
