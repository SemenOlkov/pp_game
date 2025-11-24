using UnityEngine;

[CreateAssetMenu(fileName = "Book Item", menuName = "Inventory/Items/NewBookItem")]
public class BookItem : ItemScriptableObject
{
    [TextArea(5, 10)]
    public string bookContent;

    private void Start()
    {
        itemType = ItemType.Book;
    }

    public override void Functionality()
    {
        base.Functionality();
        
        Debug.Log($"Reading book: {itemName}");
        Debug.Log($"Content: {bookContent}");
        
        OpenBookUI();
    }

    private void OpenBookUI()
    {
        // Находим менеджер ввода в сцене
        BookInputManager inputManager = FindObjectOfType<BookInputManager>();
        if (inputManager != null)
        {
            inputManager.OpenBookUI(this);
        }
        else
        {
            Debug.LogError("BookInputManager not found in scene!");
        }
    }
}