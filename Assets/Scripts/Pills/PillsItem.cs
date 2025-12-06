using UnityEngine;

[CreateAssetMenu(fileName = "Pills Item", menuName = "Inventory/Items/PillsItem")]
public class PillsItem : ItemScriptableObject
{

    private void Start()
    {
        itemType = ItemType.Pills;
        isConsumable = true;
    }

    public override void Functionality()
    {
        PlayerStatus playerStatus = FindObjectOfType<PlayerStatus>();
        if (playerStatus != null)
        {
            playerStatus.Sanity = 100;
        }
    }
}