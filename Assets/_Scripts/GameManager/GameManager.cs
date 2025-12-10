using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    // to apply the complition requirements for current level
    public static bool isGameOver = false;

    public int curretnLevel = 1;
    public int totalLevels = 1;
    [SerializeField]private TMP_Text requiredScoreText;
    //required scroe to win current level
    [SerializeField] private LevelCompleteRequirement[] levelCompleteRequirements;

   
    public static GameManager instance;

    private void Awake()
    {
        //ensure only one instance of GameManager exists
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); //persist across scenes
        }
        else
        {
            Destroy(gameObject); //destroy duplicate
        }
    }

    public void OnEnable()
    {
        PlayerInventory.OnInventoryChanged += CheckLevelComplete;
        VisionDetector.OnPlayerSpotted += OnGameOver;
        FSMWithProb.OnPlayerCaught += OnGameOver;
    }

    public void OnDisable()
    {
        PlayerInventory.OnInventoryChanged -= CheckLevelComplete;
        VisionDetector.OnPlayerSpotted -= OnGameOver;
        FSMWithProb.OnPlayerCaught -= OnGameOver;
    }
    private void Start()
    {
        requiredScoreText.text = "Required Score: " + levelCompleteRequirements[curretnLevel-1].requiredScore.ToString();
        curretnLevel = SceneManager.GetActiveScene().buildIndex;
        totalLevels = levelCompleteRequirements.Length;
    }
    private void LoadNextLevel()
    {
        if (curretnLevel >= totalLevels)
        {
            //game complete
            OnGameComplete();
            return;
        }
        curretnLevel++;
        UiManager.instance.HideCurrentActivePanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        requiredScoreText.text = "Required Score: " + levelCompleteRequirements[curretnLevel - 1].requiredScore.ToString();
    }

    private void CheckLevelComplete(int currentPlayerScore)
    {
        if(currentPlayerScore >= levelCompleteRequirements[curretnLevel-1].requiredScore)
        {
            LoadNextLevel();
        }
    }

    private void OnGameComplete()
    {
        //pop up level complete UI, play sound, etc.
        Time.timeScale = 0f; //pause the game
        UiManager.instance.OnGameComplete();
    }

    //called when player is detected by enemy
    private void OnGameOver()
    {
        //pop up game over UI, play sound, etc.
        isGameOver = true;
        Time.timeScale = 0f; //pause the game
        UiManager.instance.OnGameOver();

    }

    private void LoadPreviousLevel() 
    { 
        curretnLevel--;
        UiManager.instance.HideCurrentActivePanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }

    public void RestartLevel()
    {
        Time.timeScale = 1f; //resume the game
        UiManager.instance.HideCurrentActivePanel();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        isGameOver = false;
    }
}
