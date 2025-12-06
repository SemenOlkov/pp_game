using UnityEngine;
using UnityEngine.UI; // Для работы с UI текстом или CanvasGroup

public class FadeInText : MonoBehaviour
{
    public float duration = 1f; // Время появления
    private Text uiText; // Или CanvasGroup canvasGroup
    private CanvasGroup canvasGroup;

    void Start()
    {
        // Если используете Text
        uiText = GetComponent<Text>();
        // Или, если используете CanvasGroup
        canvasGroup = GetComponent<CanvasGroup>();

        // Изначально скрыт
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 0;
        }
        else if (uiText != null)
        {
            Color c = uiText.color;
            c.a = 0;
            uiText.color = c;
        }

        StartCoroutine(FadeIn());
    }

    private System.Collections.IEnumerator FadeIn()
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float alpha = Mathf.Clamp01(elapsed / duration);
            if (canvasGroup != null)
            {
                canvasGroup.alpha = alpha;
            }
            else if (uiText != null)
            {
                Color c = uiText.color;
                c.a = alpha;
                uiText.color = c;
            }
            yield return null;
        }
    }
}