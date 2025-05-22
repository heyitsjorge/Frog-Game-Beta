using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public AudioClip SlashingSound;
    private AudioSource audioSource;

    void Start(){
        audioSource = GameObject.Find("AudioController").GetComponent<AudioSource>();
        GetComponent<Button>().onClick.AddListener(PlaySlashSound);
    }

    void PlaySlashSound(){
        audioSource.PlayOneShot(SlashingSound);
    }

}
