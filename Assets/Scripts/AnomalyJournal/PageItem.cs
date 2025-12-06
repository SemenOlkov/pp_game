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
                // Check if this sprite is not already in the journal
                if (!journalManager.pageSprites.Contains(pageSprite))
                {
                    // Add the sprite to the journal
                    journalManager.pageSprites.Add(pageSprite);
                    
                    // Optional: Update the journal display if it's currently open
                    // journalManager.UpdateButtons();
                    
                    Debug.Log($"Page added to journal: {itemName}");
                    
                    // Optional: Show a notification or UI feedback
                    ShowPageAddedNotification();
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

    private void ShowPageAddedNotification()
    {
        // You can implement UI feedback here
        // For example, show a message or play a sound
        Debug.Log($"New journal page discovered: {itemName}");
        
        // Optional: Trigger an event for UI updates
        // EventSystem.Instance?.TriggerEvent("JournalUpdated");
    }
}