using UnityEngine;
using UnityEngine.SceneManagement;

public class SkipCutsceneButton : MonoBehaviour
{
    public void SkipCutscene()
    {
        SceneManager.LoadScene("Level1");
    }
}
