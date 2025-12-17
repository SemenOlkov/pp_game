using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class MainMenuManager : MonoBehaviour
{
    public Button continueButton;

    void Start()
    {
        // Проверяем, существует ли автосейв, чтобы знать, активировать ли кнопку
        string path = Path.Combine(Application.persistentDataPath, "Autosave.json");
        
        if (continueButton != null)
        {
            // Кнопка активна только если файл существует
            continueButton.interactable = File.Exists(path);
        }
    }

    // Метод, который нужно назначить кнопке "Продолжить" в Инспекторе
    public void ContinueGame()
    {
        if (SceneHistory.Instance != null)
        {
            SceneHistory.Instance.LoadFromFile("Autosave");
        }
    }

    // Метод для кнопки "Новая игра" (если нужно)
    public void NewGame(string firstSceneName)
    {
        // Очищаем старые данные перед началом новой игры
        if (SceneHistory.Instance != null)
        {
            SceneHistory.Instance.inventorySnapshot.Clear();
            SceneHistory.Instance.savedJournalPages.Clear();
            SceneHistory.Instance.savedSanity = 100;
            SceneHistory.Instance.savedFlashlightBattery = 100f;
            
            UnityEngine.SceneManagement.SceneManager.LoadScene(firstSceneName);
        }
    }
}