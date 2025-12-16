using UnityEngine;

[CreateAssetMenu(fileName = "Page Item", menuName = "Inventory/Items/PageItem")]
public class PageItem : ItemScriptableObject
{
    [Header("Page Settings")]
    [Tooltip("The sprite that represents this journal page")]
    public Sprite pageSprite;

    private void Start()
    {
        itemType = ItemType.Page;
        isConsumable = true;
    }

    public override void Functionality()
    {
        // Find the JournalPagesManager in the scene
        JournalPagesManager journalManager = FindObjectOfType<JournalPagesManager>();
        
        if (journalManager != null)
        {
            // Check if the sprite is not null
            if (pageSprite != null)
            {
            if (!journalManager.collectedPages.Contains(pageSprite))
            {
                journalManager.collectedPages.Add(pageSprite);
                // Также добавляем в основной список для отображения
                if (!journalManager.pageSprites.Contains(pageSprite))
                {
                    journalManager.pageSprites.Add(pageSprite);
                }
                Debug.Log($"Page added to collectedPages: {itemName}");
}
                else
                {
                    Debug.Log($"This page is already in the journal: {itemName}");
                }
            }
            else
            {
                Debug.LogWarning($"PageItem {itemName} has no sprite assigned!");
            }
        }
        else
        {
            Debug.LogError("JournalPagesManager not found in the scene!");
        }
        
    }
}