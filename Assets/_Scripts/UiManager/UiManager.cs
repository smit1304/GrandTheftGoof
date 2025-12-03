using UnityEngine;

public class UiManager : MonoBehaviour
{
    [SerializeField] private GameObject levelCompletePanel;
    [SerializeField] private GameObject gameOverPanel;
    private GameObject currentActivePanel;

    public static UiManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnGameOver()
    {
        Debug.Log("Game Over! Displaying UI...");
        gameOverPanel.SetActive(true);
        currentActivePanel = gameOverPanel;
    }

    // Called when the player chooses to restart the level through ui button
    public void OnRestartLevel()
    {
        GameManager.instance.RestartLevel();
    }

    public void OnGameComplete() 
    { 
        Debug.Log("Game Complete! Displaying UI...");
        levelCompletePanel.SetActive(true);
        currentActivePanel = levelCompletePanel;
    }

    public void HideCurrentActivePanel()
    {
        if (currentActivePanel != null)
            currentActivePanel.SetActive(false);
    }
}
