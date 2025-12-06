using UnityEngine;

[CreateAssetMenu(fileName = "Book Item", menuName = "Inventory/Items/NewBookItem")]
public class BookItem : ItemScriptableObject
{

    private void Start()
    {
        itemType = ItemType.Book;
    }

    public override void Functionality()
    {
        base.Functionality();
        
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