using UnityEngine;
using UnityEngine.UI;

public class HealthBarUI : MonoBehaviour
{
    [Header("References")]
    public Boss_Health target;              // assign in inspector
    public Camera    uiCamera;             // assign or leave null for Camera.main
    public Image     backgroundImage;      // drag in the Background Image here

    [Header("Flash Settings")]
    public Color normalColor = new Color(1f, 0f, 0f, 1f);     // red
    public Color flashColor  = new Color(1f, 0.5f, 0.5f, 1f); // lighter red
    public float flashDuration = 0.2f;

    private Slider       slider;
    private RectTransform rectTransform;
    private Image        fillImage;
    private float        lastHealth;
    private float        flashTimer;
    public float barHeight = 0.2f;

    void Awake()
    {
        slider = GetComponent<Slider>();
        rectTransform = GetComponent<RectTransform>();
        rectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, barHeight);
        fillImage = slider.fillRect.GetComponent<Image>();

        // initial colors
        fillImage.color = normalColor;
        if (backgroundImage != null)
            backgroundImage.color = normalColor;

        if (uiCamera == null)
            uiCamera = Camera.main;

        // set slider range
        slider.maxValue = target.maxHP;
        slider.value    = target.currHP;
        lastHealth      = target.currHP;
    }

    void LateUpdate()
    {
        // 1) Update position
        Vector3 screenPos = uiCamera.WorldToScreenPoint(target.transform.position);
        rectTransform.position = screenPos + Vector3.up * 50f; // adjust Y offset

        // 2) Update slider value
        float current = target.currHP;
        slider.value = current;

        // 3) Flash logic
        if (current < lastHealth)
            flashTimer = flashDuration;

        if (flashTimer > 0f)
        {
            float t = flashTimer / flashDuration;  
            fillImage.color = Color.Lerp(normalColor, flashColor, t);
            flashTimer -= Time.deltaTime;
        }
        else
        {
            fillImage.color = normalColor;
        }

        lastHealth = current;
    }
}
