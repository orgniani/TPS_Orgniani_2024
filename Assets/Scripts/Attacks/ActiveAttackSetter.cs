using UnityEngine;

public class ActiveAttackSetter : MonoBehaviour
{
    public enum ActiveWeapon { GUN = 0, EXTINGUISHER, HANDS }
    private ActiveWeapon activeWeapon;

    private ShooterController shooterController;
    private FireExtinguisherController extinguisherController;

    private void Awake()
    {
        shooterController = GetComponent<ShooterController>();
        extinguisherController = GetComponent<FireExtinguisherController>();
    }
    private void Start()
    {
        HandleChangeActiveWeapon(ActiveWeapon.HANDS);
    }

    public void HandleChangeActiveWeapon(ActiveWeapon currentActiveWeapon)
    {
        activeWeapon = currentActiveWeapon;
        shooterController.SetActiveWeapon(currentActiveWeapon);
        extinguisherController.SetActiveWeapon(currentActiveWeapon);
    }

    public ActiveWeapon GetActiveWeapon()
    {
        return activeWeapon;
    }
}
