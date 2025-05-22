using UnityEngine;
using UnityEngine.SceneManagement;

public class ToriGate : MonoBehaviour
{
    public AudioClip VictorySound;
    private AudioSource audioSource;
    public string nextSceneName = "ClosingScence";
    public float delayBeforeLoad = 5f; // Delay in seconds before loading the next scene
    private bool levelCompleted = false;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        audioSource = GetComponent<AudioSource>();

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        
    }

    // Update is called once per frame
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (levelCompleted) return;

        if (other.CompareTag("Player"))
        {
            levelCompleted = true;

            // Stop music
            GameObject musicObject = GameObject.Find("Level1MusicPlayer"); // name of your BGM source
            if (musicObject != null)
            {
                AudioSource musicSource = musicObject.GetComponent<AudioSource>();
                if (musicSource != null)
                {
                    musicSource.Stop();
                }
            }

            // Play completion sound
            if (VictorySound != null)
            {
                audioSource.PlayOneShot(VictorySound);
            }

            // Start scene transition after delay
            Invoke("LoadNextScene", delayBeforeLoad);
        }
    }

    void LoadNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }
}
    
