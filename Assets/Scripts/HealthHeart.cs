using UnityEngine;
using UnityEngine.UI;

public class HealthHeart : MonoBehaviour
{
    public Sprite fullHeart, emptyHeart;
    private Image heartImage;
    private void Awake()
    {
        heartImage = GetComponent<Image>();
    }

    public void setHeartState(bool full)
    {
        if (full)
        {
            heartImage.sprite = fullHeart;
        }
        else
        {
            heartImage.sprite = emptyHeart;
        }
    }
}
