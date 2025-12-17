using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    public ItemScriptableObject item;
    public int amount;
    public bool isEmpty = true;
    public GameObject itemIcon;
    public TMP_Text itemAmountText;

    // ДОБАВИТЬ: Флаг для различия типов слотов (нужно проставить в инспекторе префаба)
    public bool isHotbarSlot; 

    private void Awake() // Используем Awake, чтобы ссылки были готовы сразу
    {
        itemIcon = transform.GetChild(0).GetChild(0).gameObject;
        itemAmountText = transform.GetChild(0).GetChild(1).GetComponent<TMP_Text>();
    }

    public void SetIcon(Sprite icon)
    {
        Image img = itemIcon.GetComponent<Image>();
        img.color = new Color(1, 1, 1, 1);
        img.sprite = icon;  
    }

    // ДОБАВИТЬ: Метод очистки, который вызывал ошибки
    public void ClearSlot()
    {
        item = null;
        amount = 0;
        isEmpty = true;
        
        Image img = itemIcon.GetComponent<Image>();
        if (img != null)
        {
            img.color = new Color(1, 1, 1, 0); // Прозрачный, если пустой
            img.sprite = null;
        }
        
        if (itemAmountText != null) itemAmountText.text = "";
    }
}