// using UnityEngine;
// using UnityEngine.UI;
// using System.Collections.Generic;

// public class JournalPagesManager : MonoBehaviour
// {
//     [Header("Список страниц журнала")]
//     public List<string> pageNames = new List<string> { 
//         "JournalFirstPage", 
//         "JournalSecondPage", 
//         "EndlessHallwayPage" 
//     };

//     [Header("Элементы UI")]
//     public Image journalImage;
//     public Button previousButton;
//     public Button nextButton;

//     private int currentPageIndex = 0;

//     void Start()
//     {
//         // Загружаем и отображаем первую страницу
//         LoadPage(currentPageIndex);
//         CheckResources();
        
//         // Добавляем обработчики для кнопок
//         previousButton.onClick.AddListener(PreviousPage);
//         nextButton.onClick.AddListener(NextPage);
        
//         // Обновляем состояние кнопок
//         UpdateButtons();
//     }

//     [ContextMenu("Проверить загрузку ресурсов")]
//     void CheckResources()
//     {
//         foreach (string pageName in pageNames)
//         {
//             string path = $"AnomalyJournalPages/{pageName}";
//             Sprite sprite = Resources.Load<Sprite>(path);
//             if (sprite != null)
//             {
//                 Debug.Log($"✓ Найдена страница: {pageName}");
//             }
//             else
//             {
//                 Debug.LogError($"✗ Не найдена страница: {pageName} по пути: {path}");
//             }
//         }
//     }

//     void LoadPage(int pageIndex)
//     {
//         if (pageIndex < 0 || pageIndex >= pageNames.Count)
//             return;

//         // Формируем путь к изображению
//         string imagePath = $"AnomalyJournalPages/{pageNames[pageIndex]}";
        
//         // Загружаем спрайт
//         Sprite pageSprite = Resources.Load<Sprite>(imagePath);
        
//         if (pageSprite != null)
//         {
//             Debug.Log($"Изображение загрузилось");
//             journalImage.sprite = pageSprite;
//         }
//         else
//         {
//             Debug.LogError($"Не удалось загрузить изображение по пути: {imagePath}");
//         }
//     }

//     public void NextPage()
//     {
//         if (currentPageIndex < pageNames.Count - 1)
//         {
//             currentPageIndex++;
//             LoadPage(currentPageIndex);
//             UpdateButtons();
//         }
//     }

//     public void PreviousPage()
//     {
//         if (currentPageIndex > 0)
//         {
//             currentPageIndex--;
//             LoadPage(currentPageIndex);
//             UpdateButtons();
//         }
//     }

//     void UpdateButtons()
//     {
//         // Обновляем состояние кнопок в зависимости от текущей страницы
//         previousButton.interactable = currentPageIndex > 0;
//         nextButton.interactable = currentPageIndex < pageNames.Count - 1;
//     }

//     // Метод для добавления новых страниц во время выполнения
//     public void AddPage(string pageName)
//     {
//         pageNames.Add(pageName);
//         UpdateButtons();
//     }

//     // Метод для перехода к конкретной странице по имени
//     public void GoToPage(string pageName)
//     {
//         int index = pageNames.IndexOf(pageName);
//         if (index != -1)
//         {
//             currentPageIndex = index;
//             LoadPage(currentPageIndex);
//             UpdateButtons();
//         }
//     }

//     void OnDestroy()
//     {
//         // Убираем обработчики при уничтожении объекта
//         if (previousButton != null)
//             previousButton.onClick.RemoveListener(PreviousPage);
//         if (nextButton != null)
//             nextButton.onClick.RemoveListener(NextPage);
//     }
// }
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class JournalPagesManager : MonoBehaviour
{
    [Header("СПРАЙТЫ СТРАНИЦ (ПЕРЕТАЩИ СЮДА)")]
    public List<Sprite> pageSprites;

    [Header("Элементы UI")]
    public Image journalImage;
    public Button previousButton;
    public Button nextButton;

    private int currentPageIndex = 0;

    void Start()
    {
        // Проверяем что спрайты есть
        if (pageSprites == null || pageSprites.Count == 0)
        {
            Debug.LogError("НЕТ СТРАНИЦ! Перетащи спрайты в pageSprites в инспекторе!");
            return;
        }

        // Показываем первую страницу
        ShowCurrentPage();
        
        // Настраиваем кнопки
        previousButton.onClick.AddListener(PreviousPage);
        nextButton.onClick.AddListener(NextPage);
        
        UpdateButtons();
    }

    void ShowCurrentPage()
    {
        if (currentPageIndex >= 0 && currentPageIndex < pageSprites.Count && pageSprites[currentPageIndex] != null)
        {
            journalImage.sprite = pageSprites[currentPageIndex];
            Debug.Log($"Показана страница {currentPageIndex + 1}/{pageSprites.Count}");
        }
    }

    public void NextPage()
    {
        if (currentPageIndex < pageSprites.Count - 1)
        {
            currentPageIndex++;
            ShowCurrentPage();
            UpdateButtons();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            ShowCurrentPage();
            UpdateButtons();
        }
    }

    void UpdateButtons()
    {
        previousButton.interactable = currentPageIndex > 0;
        nextButton.interactable = currentPageIndex < pageSprites.Count - 1;
    }
}