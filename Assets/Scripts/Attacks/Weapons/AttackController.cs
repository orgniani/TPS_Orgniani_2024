using Cinemachine;
using StarterAssets;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class AttackController : MonoBehaviour
{
    [Header("Aiming")]
    [Header("References")]
    [SerializeField] private Rig aimRig;
    [SerializeField] private CinemachineVirtualCamera aimVirtualCamera;
    [SerializeField] protected Transform debugTransform;

    [Header("Parameters")]
    [SerializeField] private float normalSensitivity;
    [SerializeField] private float aimSensitivity;

    [SerializeField] private LayerMask aimColliderLayerMask = new LayerMask();

    protected StarterAssetsInputs starterAssetInputs;
    protected ThirdPersonController thirdPersonController;
    protected Animator animator;

    // animation IDs
    protected int animIDGun;
    protected int animIDExtinguisher;
    protected int animIDDrag;

    protected bool hasAnimator;

    private Vector3 mouseWorldPosition;
    private float aimRigWeight;

    private void Awake()
    {
        starterAssetInputs = GetComponent<StarterAssetsInputs>();
        thirdPersonController = GetComponent<ThirdPersonController>();
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        hasAnimator = TryGetComponent(out animator);
        AssignAnimationIDs();
    }

    public void HandlePlayerAiming()
    {
        aimRig.weight = Mathf.Lerp(aimRig.weight, aimRigWeight, Time.deltaTime * 20f);

        mouseWorldPosition = Vector3.zero;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);

        if (Physics.Raycast(ray, out RaycastHit raycastHit, 999f, aimColliderLayerMask))
        {
            if(debugTransform) debugTransform.position = raycastHit.point;
            mouseWorldPosition = raycastHit.point;
        }

        if (starterAssetInputs.aim)
        {
            Aiming();
        }

        else
        {
            StopAiming();
        }
    }

    private void Aiming()
    {
        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(1), 1f, Time.deltaTime * 10f));
        aimRigWeight = 1f;

        thirdPersonController.SetRotateOnMove(false);
        thirdPersonController.SetSprintOnAim(false);
        thirdPersonController.SetStrafeOnAim(true);

        if (!aimVirtualCamera) return;

        aimVirtualCamera.gameObject.SetActive(true);
        thirdPersonController.SetSensitivity(aimSensitivity);

        Vector3 worldAimTarget = mouseWorldPosition;
        worldAimTarget.y = transform.position.y;
        Vector3 aimDirection = (worldAimTarget - transform.position).normalized;

        transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * 20f);
    }

    private void StopAiming()
    {
        animator.SetLayerWeight(1, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        animator.SetLayerWeight(2, Mathf.Lerp(animator.GetLayerWeight(1), 0f, Time.deltaTime * 10f));
        aimRigWeight = 0f;

        thirdPersonController.SetRotateOnMove(true);
        thirdPersonController.SetStrafeOnAim(false);

        if (!aimVirtualCamera) return;

        aimVirtualCamera.gameObject.SetActive(false);
        thirdPersonController.SetSensitivity(normalSensitivity);
    }

    public virtual void Shoot()
    {
    }

    private void AssignAnimationIDs()
    {
        animIDExtinguisher = Animator.StringToHash("Extinguisher");
        animIDGun = Animator.StringToHash("Gun");
        animIDDrag = Animator.StringToHash("Drag");
    }
}
