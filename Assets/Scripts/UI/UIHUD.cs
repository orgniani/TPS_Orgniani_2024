using StarterAssets;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [Header("UI References")]
    [Header("Gun")]
    [SerializeField] private Image crossHair;

    [Header("Player References")]
    [SerializeField] private ShooterController shootController;
    [SerializeField] private StarterAssetsInputs starterAssetInputs;

    private void Update()
    {
        CrossHairAppear();
    }

    private void ChangeGunSightColor()
    {
        if (!crossHair) return;

        Color originalColor = crossHair.color;

        Color newColor = shootController.IsPointingAtEnemy ? new Color(1, 0, 0, originalColor.a) : new Color(1, 1, 1, originalColor.a);

        crossHair.color = newColor;
    }

    private void CrossHairAppear()
    {
        if (!crossHair) return;

        if(starterAssetInputs.aim)
        {
            crossHair.gameObject.SetActive(true);
            ChangeGunSightColor();
        }

        else
        {
            crossHair.gameObject.SetActive(false);
        }
    }
}
