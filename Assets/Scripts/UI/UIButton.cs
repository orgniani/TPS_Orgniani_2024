using UnityEngine;
using UnityEngine.EventSystems;

public class UIButton : MonoBehaviour
{
    [Header("Event System")]
    [SerializeField] private EventSystem eventSystem;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip hoverButton;
    [SerializeField] private AudioClip clickButton;

    public void HoverSound()
    {
        if(hoverButton)
            audioSource.PlayOneShot(hoverButton);
    }

    public void ClickSound()
    {
        if(clickButton)
            audioSource.PlayOneShot(clickButton);
    }

    public void SetMainButton(GameObject mainButton)
    {
        eventSystem.SetSelectedGameObject(mainButton);
    }
}
