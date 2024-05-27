using StarterAssets;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHUD : MonoBehaviour
{
    [Header("UI References")]
    [Header("Gun")]
    [SerializeField] private Image crossHair;

    [Header("Health")]
    [SerializeField] private TMP_Text HPText;
    [SerializeField] private Image healthBar;

    [Header("Player References")]
    [SerializeField] private HealthController playerHP;
    [SerializeField] private ShooterController shootController;
    [SerializeField] private StarterAssetsInputs starterAssetInputs;

    private void OnEnable()
    {
        playerHP.onHurt += HandleHPBar;
    }

    private void OnDisable()
    {
        playerHP.onHurt -= HandleHPBar;
    }
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

    private void HandleHPBar()
    {
        if (!HPText || !healthBar) return;

        HPText.text = playerHP.Health + "%";
        healthBar.fillAmount = 1.0f * playerHP.Health / playerHP.MaxHealth;
    }
}
