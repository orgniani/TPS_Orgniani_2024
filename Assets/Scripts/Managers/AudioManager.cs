using UnityEngine;

public class AudioManager : MonoBehaviour
{
    private static AudioManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this);
        }

        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        if (!PlayerPrefs.HasKey(PrefsKeys.VolumeKey))
        {
            PlayerPrefs.SetFloat(PrefsKeys.VolumeKey, 1);
            AudioListener.volume = 1;
        }

        else
        {
            AudioListener.volume = PlayerPrefs.GetFloat(PrefsKeys.VolumeKey);
        }
    }
}
