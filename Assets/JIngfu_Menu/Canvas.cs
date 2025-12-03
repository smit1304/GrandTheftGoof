using UnityEngine;

public class Canvas : MonoBehaviour
{
    public static Canvas instance;

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

    public void QuitGames()
    {
#if UNITY_EDITOR
        
        UnityEditor.EditorApplication.isPlaying = false;
#else
       
        Application.Quit();
#endif
    }
}