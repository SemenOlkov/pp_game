using UnityEngine;
using System.Collections;

public class StartFromAutosave : MonoBehaviour
{
    public void OnButtonClick()
    {
        // Проверяем, существует ли глобальный менеджер истории
        if (SceneHistory.Instance == null)
        {
            Debug.LogError("[StartFromAutosave] SceneHistory не найден на сцене! Убедитесь, что он создан в первой сцене игры.");
            return;
        }

        string sceneToLoad = SceneHistory.Instance.LastGameplayScene;

        if (!string.IsNullOrEmpty(sceneToLoad))
        {
            Debug.Log($"[StartFromAutosave] Запуск рестарта на сцену: {sceneToLoad}");
            
            // КРИТИЧЕСКИ ВАЖНО: Запускаем корутину на БЕССМЕРТНОМ объекте SceneHistory,
            // чтобы она не прервалась при уничтожении кнопки/сцены смерти.
            SceneHistory.Instance.StartCoroutine(SceneHistory.Instance.GlobalLoadRoutine(sceneToLoad));
        }
        else
        {
            Debug.LogWarning("[StartFromAutosave] Нет сохраненной сцены в истории!");
        }
    }
}