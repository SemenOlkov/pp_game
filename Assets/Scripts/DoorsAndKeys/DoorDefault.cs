using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class DoorDefault : MonoBehaviour
{
    [Header("Door Settings")]
    public bool canGo = false;
    public string sceneName = "DebugScene";
    public bool requiresKey = false;
    public string requiredKeyCode = "1";

    [Header("UI Settings")]
    [SerializeField] private GameObject cantGoText;    // Канвас 1: Нельзя (нет ключа)
    [SerializeField] private GameObject canGoText;     // Канвас 2: Можно (есть ключ/не нужен)
    [SerializeField] private GameObject crowbarHintText; // Канвас 3: Взломать (есть монтировка)

    [Header("Raycast Settings")]
    [SerializeField] private float interactionDistance = 7f;
    [SerializeField] private LayerMask interactionLayer;

    private bool isHovered = false;

    // Ссылки
    private Camera PlayerCamera => Camera.main;
    private HotbarPanel CurrentHotbar => PersistentObject.Instance != null 
        ? PersistentObject.Instance.GetComponentInChildren<HotbarPanel>(true) 
        : null;

    void Start()
    {
        HideAllHints();
    }

    void Update()
    {
        HandleRaycast();
        HandleInput();
    }

    private void HandleRaycast()
    {
        Camera cam = PlayerCamera;
        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                isHovered = true;
                UpdateHints();
                return;
            }
        }
        
        if (isHovered)
        {
            isHovered = false;
            HideAllHints();
        }
    }

    private void UpdateHints()
    {
        HideAllHints();

        bool hasKey = CheckActiveKey();
        bool hasCrowbar = IsHoldingCrowbar();

        // 1. Приоритет: Если нужен ключ, дверь открыта (canGo), но в руках монтировка
        if (canGo && requiresKey && !hasKey && hasCrowbar)
        {
            if (crowbarHintText != null) crowbarHintText.SetActive(true);
        }
        // 2. Если дверь закрыта программно ИЛИ нужен ключ (а в руках нет ни ключа, ни монтировки)
        else if (!canGo || (requiresKey && !hasKey))
        {
            if (cantGoText != null) cantGoText.SetActive(true);
        }
        // 3. Можно войти (есть ключ или он не нужен)
        else
        {
            if (canGoText != null) canGoText.SetActive(true);
        }
    }

    private void HandleInput()
    {
        if (!isHovered) return;

        // Обычный вход (Enter)
        if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            TryLoadScene();
        }

        // Взлом монтировкой (ЛКМ)
        if (Input.GetMouseButtonDown(0) && canGo && requiresKey && IsHoldingCrowbar() && !CheckActiveKey())
        {
            TryBreakLock();
        }
    }

    private void TryBreakLock()
    {
        if (PlayerStatus.Instance != null)
        {
            // Проверяем, хватит ли психики на штраф
            if (PlayerStatus.Instance.Sanity > 10)
            {
                PlayerStatus.Instance.Sanity -= 10;
                Debug.Log("Взлом! Штраф -10 психики. Переход...");
                SceneManager.LoadScene(sceneName);
            }
            else
            {
                Debug.Log("Слишком страшно взламывать, психики недостаточно!");
                // Можно добавить звук испуга или текст "Я не справлюсь"
            }
        }
    }

    private bool CheckActiveKey()
    {
        HotbarPanel hb = CurrentHotbar;
        if (hb == null) return false;

        var item = hb.GetSelectedItem();
        if (item != null && item.itemType == ItemType.Key)
        {
            KeyItem key = item as KeyItem;
            return key != null && key.keyCode == requiredKeyCode;
        }
        return false;
    }

    private bool IsHoldingCrowbar()
    {
        HotbarPanel hb = CurrentHotbar;
        if (hb == null) return false;

        var item = hb.GetSelectedItem();
        return item != null && item.itemType == ItemType.Crowbar;
    }

    private void TryLoadScene()
    {
        if (canGo && (!requiresKey || CheckActiveKey()))
        {
            if (!string.IsNullOrEmpty(sceneName))
                SceneManager.LoadScene(sceneName);
        }
    }

    private void HideAllHints()
    {
        if (canGoText != null) canGoText.SetActive(false);
        if (cantGoText != null) cantGoText.SetActive(false);
        if (crowbarHintText != null) crowbarHintText.SetActive(false);
    }
}