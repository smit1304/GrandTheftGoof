using UnityEngine;
using UnityEngine.UI;

public class OptionsMenu : MonoBehaviour
{
    public Toggle musicToggle;
    public Toggle sfxToggle;

    private void Start()
    {
        if (AudioManager.Instance != null)
        {
            musicToggle.isOn = AudioManager.Instance.musicOn;
            sfxToggle.isOn = AudioManager.Instance.sfxOn;
        }
        else
        {
            musicToggle.isOn = true;
            sfxToggle.isOn = true;
        }
    }

    // 
    public void OnMusicToggleChanged(bool value)
    {
        Debug.Log("Music Toggle Changed: " + value);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicEnabled(value);
        }
    }

    public void OnSfxToggleChanged(bool value)
    {
        Debug.Log("SFX Toggle Changed: " + value);
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSfxEnabled(value);
        }
    }
  
}
