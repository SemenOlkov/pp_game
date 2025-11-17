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

    private void Start()
    {
        itemIcon = transform.GetChild(0).gameObject;
        itemAmountText = transform.GetChild(1).GetComponent<TMP_Text>();
    }

    public void SetIcon(Sprite icon)
    {
        itemIcon.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        itemIcon.GetComponent<Image>().sprite = icon;  
    }
}
