using UnityEngine;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    [Header("Основные характеристики")]
    [SerializeField] private int health = 100;
    [SerializeField] private int sanity = 100;
    
    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI sanityText;
    
    private const int MAX_VALUE = 100;

    public int Health
    {
        get => health;
        set
        {
            health = Mathf.Clamp(value, 0, MAX_VALUE);
            UpdateUI();
        }
    }
    
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
        if (healthText != null)
            healthText.text = $"Здоровье: {health}/{MAX_VALUE}";
        
        if (sanityText != null)
            sanityText.text = $"Психика: {sanity}/{MAX_VALUE}";
    }
}