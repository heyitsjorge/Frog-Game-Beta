using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class ClosingScene : MonoBehaviour
{
    public TextMeshProUGUI Dialogue;   
    public float textDisplayTime = 2f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        StartCoroutine(ShowDialogLines(new string[]{
            "Thanks For Playing!",
            "Jiri's Leap!"
        }));
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
    yield return new WaitForSeconds(1f);
    SceneManager.LoadScene("Main Menu");
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

}
