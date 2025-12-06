using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class BlueprintPagesManager : MonoBehaviour
{
    [Header("СПРАЙТЫ СТРАНИЦ ПО УМОЛЧАНИЮ (для теста)")]
    public List<Sprite> defaultPages;

    [Header("Элементы UI")]
    public Image blueprintImage;
    public Button previousButton;
    public Button nextButton;
    public Text pageCounterText;

    private List<Sprite> currentPages = new List<Sprite>();
    private int currentPageIndex = 0;

    void Start()
    {
        // Настраиваем кнопки
        if (previousButton != null)
            previousButton.onClick.AddListener(PreviousPage);
        
        if (nextButton != null)
            nextButton.onClick.AddListener(NextPage);
        
        // Если есть дефолтные страницы, используем их для теста
        if (defaultPages != null && defaultPages.Count > 0)
        {
            currentPages = new List<Sprite>(defaultPages);
            ShowCurrentPage();
            UpdateButtons();
        }
        
        UpdatePageCounter();
    }

    public void SetPages(Sprite[] pages)
    {
        if (pages == null || pages.Length == 0)
        {
            Debug.LogWarning("НЕТ СТРАНИЦ В BLUEPRINT! Используются дефолтные.");
            if (defaultPages != null && defaultPages.Count > 0)
            {
                currentPages = new List<Sprite>(defaultPages);
            }
            else
            {
                Debug.LogError("НЕТ СТРАНИЦ! Добавьте спрайты в BlueprintItem или в defaultPages!");
                return;
            }
        }
        else
        {
            currentPages = new List<Sprite>(pages);
        }

        currentPageIndex = 0;
        ShowCurrentPage();
        UpdateButtons();
        
        // Активируем UI если он был выключен
        gameObject.SetActive(true);
    }

    void ShowCurrentPage()
    {
        if (currentPages.Count > 0 && currentPageIndex >= 0 && currentPageIndex < currentPages.Count)
        {
            if (blueprintImage != null && currentPages[currentPageIndex] != null)
            {
                blueprintImage.sprite = currentPages[currentPageIndex];
                Debug.Log($"Показана страница {currentPageIndex + 1}/{currentPages.Count}");
            }
        }
        else
        {
            Debug.LogWarning("Не могу показать страницу: индекс вне диапазона или нет страниц");
        }
        
        UpdatePageCounter();
    }

    void UpdatePageCounter()
    {
        if (pageCounterText != null)
        {
            if (currentPages.Count > 0)
            {
                pageCounterText.text = $"{currentPageIndex + 1}/{currentPages.Count}";
            }
            else
            {
                pageCounterText.text = "0/0";
            }
        }
    }

    void UpdateButtons()
    {
        if (previousButton != null)
            previousButton.interactable = currentPageIndex > 0;
        
        if (nextButton != null)
            nextButton.interactable = currentPageIndex < currentPages.Count - 1;
    }

    public void NextPage()
    {
        if (currentPageIndex < currentPages.Count - 1)
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

    // Метод для сброса к первой странице
    public void ResetToFirstPage()
    {
        currentPageIndex = 0;
        ShowCurrentPage();
        UpdateButtons();
    }
}