using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class SanityEventsManager : MonoBehaviour
{
    [Header("UI Settings")]
    [SerializeField] private GameObject blackPanel; // Панель, которая перекроет экран
    [SerializeField] private float fadeAlpha = 1f;    // Целевая прозрачность (1 = полностью черный)

    [Header("Scene Settings")]
    [SerializeField] private string targetSceneName = "42_part_two";

    private bool isEventTriggered = false;

    void Start()
    {
        // Скрываем панель на старте
        if (blackPanel != null)
            blackPanel.SetActive(false);

        // Устанавливаем психику на 42 при старте сцены
        StartCoroutine(SetInitialSanity());
    }

    private IEnumerator SetInitialSanity()
    {
        // Ждем один кадр, чтобы Instance успел инициализироваться в Awake
        yield return null;

        if (PlayerStatus.Instance != null)
        {
            PlayerStatus.Instance.Sanity = 42;
            Debug.Log("Стартовая психика установлена на 42");
        }
    }

    void Update()
    {
        // Если событие еще не произошло, проверяем значение психики
        if (!isEventTriggered && PlayerStatus.Instance != null)
        {
            if (PlayerStatus.Instance.Sanity == 1)
            {
                isEventTriggered = true; // Чтобы не запускать многократно
                StartCoroutine(ResurrectRoutine());
            }
        }
    }

    private IEnumerator ResurrectRoutine()
    {
        Debug.Log("Психика упала до 1! Запуск события...");

        // 1. Восстанавливаем психику до 100
        PlayerStatus.Instance.Sanity = 100;

        // 2. Показываем черную панель
        if (blackPanel != null)
        {
            blackPanel.SetActive(true);
            Image img = blackPanel.GetComponent<Image>();
            if (img != null)
            {
                img.color = new Color(0, 0, 0, fadeAlpha);
            }
        }

        // 3. Ждем 7 секунд
        yield return new WaitForSeconds(7f);

        // 4. Загружаем сцену
        Debug.Log($"Загрузка сцены: {targetSceneName}");
        SceneManager.LoadScene(targetSceneName);
    }
}