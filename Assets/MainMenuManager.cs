using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    public Button startButton;
    public Button exitButton;
    public Button creditButton;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startButton.onClick.AddListener(StartGame);
        exitButton.onClick.AddListener(QuitGame);
        creditButton.onClick.AddListener(ShowCredits);
    }

    public void StartGame()
{
    StartCoroutine(DelayedIntro());
}

private System.Collections.IEnumerator DelayedIntro()
{
    yield return new WaitForSeconds(1.2f); // Wait for audio to finish
    SceneManager.LoadScene("IntroCutScene");
}

private System.Collections.IEnumerator DelayedCredits()
{
    yield return new WaitForSeconds(1.2f); // Wait for audio to finish
    SceneManager.LoadScene("Credit Scene");
}


private void ShowCredits()
{
    StartCoroutine(DelayedCredits());
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
}
