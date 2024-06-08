using StarterAssets;
using System;
using UnityEngine;

public class PlayerObjectPicker : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private AttackSwapController attackSwapController;
    [SerializeField] private LayerMask triggerLayer;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip pickUpItemSound;

    private HealthController HP;
    private ShooterController gun;
    private StarterAssetsInputs starterAssetsInputs;

    public event Action onPickUp = delegate { };

    private void Awake()
    {
        HP = GetComponent<HealthController>();
        gun = GetComponent<ShooterController>();
        starterAssetsInputs = GetComponent<StarterAssetsInputs>();
    }

    private void OnTriggerEnter(Collider other)
    {
        PickUpItems item = other.GetComponent<PickUpItems>();
        if (item == null) return;

        if (triggerLayer == (triggerLayer | (1 << other.gameObject.layer)))
        {
            switch (item.GetItemType())
            {
                case ItemType.AMMO:
                    if (gun.AmmoAmount >= gun.MaxAmmoAmount || !attackSwapController.AquiredGun) return;

                    audioSource.PlayOneShot(pickUpItemSound);

                    gun.ReplenishAmmo();
                    item.gameObject.SetActive(false);
                    break;

                case ItemType.LIFE:

                    if (HP.Health >= HP.MaxHealth) return;

                    audioSource.PlayOneShot(pickUpItemSound);

                    HP.RestoreHP(item.GetRestoredHP());
                    break;

                case ItemType.EXTINGUISHER:

                    attackSwapController.AquiredExtinguisher = true;
                    starterAssetsInputs.fireExtinguisher = true;

                    onPickUp?.Invoke();
                    break;

                case ItemType.GUN:

                    attackSwapController.AquiredGun = true;
                    starterAssetsInputs.gun = true;

                    onPickUp?.Invoke();
                    break;
            }
        }

        item.gameObject.SetActive(false);
    }
}
