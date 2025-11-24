using UnityEngine;

public enum Difficulty
{
    Easy = 0,
    Medium = 1,
    Hard = 2
}

public class GameSettings : MonoBehaviour
{
    public static GameSettings Instance { get; private set; }

    public bool musicOn = true;
    public bool sfxOn = true;
    public Difficulty difficulty = Difficulty.Easy;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        
        musicOn = PlayerPrefs.GetInt("MusicOn", 1) == 1;
        sfxOn = PlayerPrefs.GetInt("SfxOn", 1) == 1;
        difficulty = (Difficulty)PlayerPrefs.GetInt("Difficulty", 0);
    }

    public void Save()
    {
        PlayerPrefs.SetInt("MusicOn", musicOn ? 1 : 0);
        PlayerPrefs.SetInt("SfxOn", sfxOn ? 1 : 0);
        PlayerPrefs.SetInt("Difficulty", (int)difficulty);
        PlayerPrefs.Save();
    }
}
