using UnityEngine;

public class LevelStartMenu : MonoBehaviour
{
    [Header("Start Menu UI ")]
    public GameObject startMenuPanel;   // 

    [Header("Pause Game")]
    public bool pauseOnStart = true;

    private void Start()
    {
        // 1. 
        if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(true);
        }

        // 2. 
        if (pauseOnStart)
        {
            Time.timeScale = 0f;
        }
    }

    // 
    public void OnPlayClicked_InLevel()
    {
        //
        if (startMenuPanel != null)
        {
            startMenuPanel.SetActive(false);
        }

        // 
        Time.timeScale = 1f;
    }
}
