using UnityEngine;
using UnityEngine.UI;

public class VendingApplyButton : MonoBehaviour
{
    [SerializeField] private VendingMachineInteraction vendingMachine;
    
    private void Start()
    {
        Button button = GetComponent<Button>();
        if (button != null && vendingMachine != null)
        {
            button.onClick.AddListener(vendingMachine.OnApplyButtonClicked);
        }
    }
}