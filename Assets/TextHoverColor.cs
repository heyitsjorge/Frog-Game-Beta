using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public class TMPTextHoverColor : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public TextMeshProUGUI targetText;
    public Color normalColor = Color.white;
    public Color hoverColor = Color.yellow;

    void Awake()
{
    if (targetText == null)
        targetText = GetComponentInChildren<TextMeshProUGUI>();
}


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (targetText != null)
            targetText.color = hoverColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (targetText != null)
            targetText.color = normalColor;
    }
}
