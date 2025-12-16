using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement; // Важно: добавляем это для работы со сценами

public class SanityEffects : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Image darkeningPanel;
    [SerializeField] private GameObject SingletonUI;

    [Header("Darkness Settings")]
    [SerializeField] private float maxDarkness = 1f;
    [SerializeField] private float minDarkness = 0f;

    [Header("Aggressive Pulse Settings (Sanity <= 50)")]
    [SerializeField] private float pulseFadeSpeed = 2f;
    [SerializeField] private float maxWaitTime = 8f;
    [SerializeField] private float minWaitTime = 1.5f;

    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;

    private PlayerStatus playerStatus;
    private float baseTargetDarkness;
    private float currentEffectDarkness;
    private int lastSanity;
    private Coroutine pulseCoroutine;


    void Start()
    {
        // Вызываем поиск ссылок также и при первом старте
        RefreshPlayerStatus(); // Используем новый метод
    
        // Настраиваем панель
        if (darkeningPanel != null)
        {
            darkeningPanel.gameObject.SetActive(true);
            SetDarknessAlpha(0f);
        }

        if (playerStatus != null)
        {
            lastSanity = playerStatus.Sanity;
            UpdateDarknessBasedOnSanity(lastSanity);
        }

        if (showDebugLogs) Debug.Log("SanityEffects initialized");
    }

    public void RefreshPlayerStatus()
    {
        playerStatus = FindObjectOfType<PlayerStatus>();
        if (playerStatus != null && showDebugLogs)
        {
            Debug.Log("SanityEffects: Ссылка на PlayerStatus обновлена.");
        }
    }   

    private void ExecuteDeath()
    {
        if (PersistentObject.Instance != null)
            Destroy(PersistentObject.Instance.gameObject);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;    
        SceneManager.LoadScene("Dead");
    }

    void Update()
    {
        if (playerStatus == null) return;

        int currentSanity = playerStatus.Sanity;

        if (currentSanity != lastSanity)
        {
            UpdateDarknessBasedOnSanity(currentSanity);
            HandlePulseRoutine(currentSanity);
            lastSanity = currentSanity;
        }

        UpdateVisuals();
    }

    private void UpdateDarknessBasedOnSanity(int sanity)
    {
        float spentSanity = (100f - sanity) / 100f;
        float multiplier = 1f;

        if (sanity > 50) multiplier = 0.2f;
        else if (sanity > 35) multiplier = 0.333f;
        else if (sanity > 15) multiplier = 0.5f;
        else if (sanity > 0) multiplier = 0.666f;
        else multiplier = 1f;

        baseTargetDarkness = Mathf.Clamp(spentSanity * multiplier, minDarkness, maxDarkness);

        // Активируем скример 
        if (sanity <= 0)
        {
            ExecuteDeath();
        }
    }

    private void HandlePulseRoutine(int sanity)
    {
        if (sanity <= 50 && pulseCoroutine == null)
        {
            pulseCoroutine = StartCoroutine(PulsingDarknessRoutine());
        }
        else if (sanity > 50 && pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
            currentEffectDarkness = 0;
        }
    }

    private IEnumerator PulsingDarknessRoutine()
    {
        while (true)
        {
            int currentSanity = playerStatus.Sanity;
            float sanityFactor = Mathf.InverseLerp(0, 50, currentSanity);
            float intervalWait = Mathf.Lerp(minWaitTime, maxWaitTime, sanityFactor);

            yield return new WaitForSeconds(intervalWait);

            while (currentEffectDarkness < 1f)
            {
                currentEffectDarkness += Time.deltaTime * pulseFadeSpeed;
                yield return null;
            }
            currentEffectDarkness = 1f;

            float blackoutDuration = GetBlackoutDuration(currentSanity);
            yield return new WaitForSeconds(blackoutDuration);

            while (currentEffectDarkness > 0f)
            {
                currentEffectDarkness -= Time.deltaTime * pulseFadeSpeed;
                yield return null;
            }
            currentEffectDarkness = 0f;
        }
    }

    private float GetBlackoutDuration(int sanity)
    {
        if (sanity <= 20) return 5.5f;
        
        if (sanity < 30)
        {
            float t = Mathf.InverseLerp(20, 30, sanity);
            return Mathf.Lerp(5.5f, 4.0f, t);
        }
        
        if (sanity < 40)
        {
            float t = Mathf.InverseLerp(30, 40, sanity);
            return Mathf.Lerp(4.0f, 3.0f, t);
        }
        
        float t2 = Mathf.InverseLerp(40, 50, sanity);
        return Mathf.Lerp(3.0f, 2.5f, t2);
    }

    private void UpdateVisuals()
    {
        if (darkeningPanel == null) return;

        float finalTarget = Mathf.Max(baseTargetDarkness, currentEffectDarkness);
        Color currentColor = darkeningPanel.color;
        float newAlpha = Mathf.Lerp(currentColor.a, finalTarget, Time.deltaTime * 5f);
        
        SetDarknessAlpha(newAlpha);
    }

    private void SetDarknessAlpha(float alpha)
    {
        Color color = darkeningPanel.color;
        color.a = Mathf.Clamp(alpha, 0, 1);
        darkeningPanel.color = color;
    }

    public void ResetEffects()
    {
        if (pulseCoroutine != null)
        {
            StopCoroutine(pulseCoroutine);
            pulseCoroutine = null;
        }
        
        baseTargetDarkness = 0f;
        currentEffectDarkness = 0f;
        
        if (darkeningPanel != null) SetDarknessAlpha(0f);
        if (playerStatus != null) lastSanity = playerStatus.Sanity;
    }
}