using UnityEngine;

[CreateAssetMenu(fileName = "Book Item", menuName = "Inventory/Items/NewBookItem")]
public class BookItem : ItemScriptableObject
{
    private void Start()
    {
        itemType = ItemType.Book;
    }
}
