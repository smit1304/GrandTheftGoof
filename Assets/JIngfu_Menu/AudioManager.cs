using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Music")]
    public AudioSource musicSource;      
    public AudioSource uiSfxSource;      
    public AudioClip uiHoverClip;        
    public AudioClip uiClickClip;        

    [Header("Switches")]
    public bool musicOn = true;          
    public bool sfxOn = true;         

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        //DontDestroyOnLoad(gameObject);  

        ApplyMusicState();
    }

    //
    public void PlayUIHover()
    {
        if (!sfxOn) return;
        if (uiSfxSource != null && uiHoverClip != null)
        {
            uiSfxSource.PlayOneShot(uiHoverClip);
        }
    }

    public void PlayUIClick()
    {
        if (!sfxOn) return;
        if (uiSfxSource != null && uiClickClip != null)
        {
            uiSfxSource.PlayOneShot(uiClickClip);
        }
    }

    //
    public void SetMusicEnabled(bool enabled)
    {
        musicOn = enabled;
        ApplyMusicState();
    }

    public void SetSfxEnabled(bool enabled)
    {
        sfxOn = enabled;
        
    }

    private void ApplyMusicState()
    {
        if (musicSource != null)
        {
            musicSource.mute = !musicOn;   
        }
    }
}
