using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    [Header("Основные характеристики")]
    [SerializeField] private int sanity = 100;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI sanityText;
    
    private const int MAX_VALUE = 100;

    
    public int Sanity
    {
        get => sanity;
        set
        {
            sanity = Mathf.Clamp(value, 0, MAX_VALUE);
            UpdateUI();
        }
    }
    
    public int MaxValue => MAX_VALUE;

    void Start()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (sanityText != null)
            sanityText.text = $"Психика: {sanity}/{MAX_VALUE}";
    }
}