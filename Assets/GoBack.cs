using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
public class GoBack : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public Button MainMenuButton;
    void Start()
    {
        MainMenuButton.onClick.AddListener(GoToMainMenu);
    }
    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
