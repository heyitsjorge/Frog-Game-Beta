using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(QuitGame);
    }

    public void StartGame()
    {
        // Load the game scene (replace "GameScene" with the actual name of your game scene)
        SceneManager.LoadScene("IntroCutScene");
    }
    public void QuitGame()
    {
        // Quit the application
        Application.Quit();

        // If running in the editor, stop playing the scene
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
