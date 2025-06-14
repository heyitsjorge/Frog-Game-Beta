using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class IntroSceneManager : MonoBehaviour
{
    public GameObject idleJiri_0;
    public GameObject fallenMaster;
    public Transform walkTarget;
    public float walkSpeed = 2f;

    public CanvasGroup FadeIn;
    public TextMeshProUGUI Dialogue;
    public float textDisplayTime = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Start the coroutine to handle the intro scene
        StartCoroutine(IntroScene());
    }

    IEnumerator IntroScene(){
        // Fade from black
        yield return StartCoroutine(FadeFromBlack());

        // Walk Jiri to the master
        yield return StartCoroutine(MoveJiri());

        // Show dialog
        yield return StartCoroutine(ShowDialogLines(new string[]{
            "Master... How Could This Happen?",
            "I Will Find The One Who Did This!",
            "I swear I will uncover the truth."
    }));

        // Pause, then fade out
        yield return new WaitForSeconds(1.5f);
        yield return StartCoroutine(FadeToBlack());

        // Load Level 1
        SceneManager.LoadScene("Level1TileSet");
    }

    IEnumerator MoveJiri(){
        while (Vector2.Distance(idleJiri_0.transform.position, walkTarget.position) > 0.1f)
        {
            idleJiri_0.transform.position = Vector2.MoveTowards(idleJiri_0.transform.position, walkTarget.position, walkSpeed * Time.deltaTime);
            yield return null;
        }
    }

    IEnumerator ShowDialogLines(string[] lines){
    Dialogue.enabled = true;

    foreach (string line in lines)
    {
        Dialogue.text = "";
        yield return StartCoroutine(TypewriterEffect(line)); // optional: add typing effect
        yield return new WaitForSeconds(textDisplayTime);
    }

    Dialogue.enabled = false;
}

    IEnumerator TypewriterEffect(string line){
    Dialogue.text = "";
    // Optional: fade in alpha first
    Color textColor = Dialogue.color;
    textColor.a = 0;
    Dialogue.color = textColor;

    while (Dialogue.color.a < 1f)
    {
        textColor.a += Time.deltaTime * 2f; // fade speed
        Dialogue.color = textColor;
        yield return null;
    }

    // Then type the text one character at a time
    for (int i = 0; i < line.Length; i++)
    {
        Dialogue.text += line[i];
        yield return new WaitForSeconds(0.03f); // type speed
    }
    }


    IEnumerator FadeFromBlack()
    {
        while (FadeIn.alpha > 0)
        {
            FadeIn.alpha -= Time.deltaTime;
            yield return null;
        }
    }
    IEnumerator FadeToBlack()
    {
        while (FadeIn.alpha < 1)
        {
            FadeIn.alpha += Time.deltaTime;
            yield return null;
        }
    }

}

