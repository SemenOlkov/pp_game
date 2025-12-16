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
    public List<Sprite> collectedPages = new List<Sprite>();

    public void RefreshJournalUI()
    {
        if (collectedPages == null) return;

        // Синхронизируем основной список спрайтов со списком сохраненных страниц
        foreach (Sprite page in collectedPages)
        {
            if (!pageSprites.Contains(page))
            {
                pageSprites.Add(page);
            }
        }

        // Сбрасываем индекс на первую страницу и обновляем вид
        currentPageIndex = 0;
        ShowCurrentPage();
        UpdateButtons();
    
        Debug.Log("Журнал: Визуальный список обновлен. Всего страниц: " + pageSprites.Count);
    }

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