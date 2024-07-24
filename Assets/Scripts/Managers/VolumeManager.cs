using UnityEngine;
using UnityEngine.UI;

public class VolumeManager : MonoBehaviour
{
    [Header("Volume")]
    [SerializeField] private Slider volumeSlider;

    [Header("Sound On/Off Icon")]
    [SerializeField] private Sprite soundOnIcon;
    [SerializeField] private Sprite soundOffIcon;
    [SerializeField] private Image soundIcon;

    private void Start()
    {
        Load();
    }

    public void ChangeVolume()
    {
        AudioListener.volume = volumeSlider.value;
        Save();

        UpdateSoundIcon(volumeSlider.value);
    }

    private void Load()
    {
        volumeSlider.value = PlayerPrefs.GetFloat(PrefsKeys.VolumeKey);
    }

    private void Save()
    {
        PlayerPrefs.SetFloat(PrefsKeys.VolumeKey, volumeSlider.value);
    }

    private void UpdateSoundIcon(float volume)
    {
        if (volume == 0) soundIcon.sprite = soundOffIcon;
        else soundIcon.sprite = soundOnIcon;
    }
}
