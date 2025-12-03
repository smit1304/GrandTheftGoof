using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;       // Panel_MainMenu
    public GameObject optionsPanel;        // Panel_Options
    public GameObject instructionsPanel;   // Panel_Instructions

    [Header("Game Scene")]
    public string gameSceneName = "Flock Test";  // 换成你的关卡 Scene 名

    private void Start()
    {
        ShowMainMenu();    // 进入场景默认显示主菜单
    }

    private void HideAllPanels()
    {
        if (mainMenuPanel != null) mainMenuPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        if (instructionsPanel != null) instructionsPanel.SetActive(false);
    }

    // ===== 菜单切换函数 =====

    public void ShowMainMenu()
    {
        HideAllPanels();
        if (mainMenuPanel != null) mainMenuPanel.SetActive(true);
    }

    public void ShowOptions()
    {
        HideAllPanels();
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }

    public void ShowInstructions()
    {
        HideAllPanels();
        if (instructionsPanel != null) instructionsPanel.SetActive(true);
    }

    

    public void OnPlayClicked()
    {
        SceneManager.LoadScene(gameSceneName);
        //HideAllPanels();
    }

    //
    public void OnOptionsClicked()
    {
        ShowOptions();
    }

    // 
    public void OnInstructionsClicked()
    {
        ShowInstructions();
    }

    // 
    public void OnBackToMainClicked()
    {
        Debug.Log("Back button clicked, try go to MainMenu");
        ShowMainMenu();
    }

    //
    public void OnExitClicked()
    {
        Application.Quit();

#if UNITY_EDITOR
        // 在 Editor 里测试时，让退出 = 停止播放
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
