using UnityEngine;
using UnityEngine.UI;

public class CrowbarInteraction : MonoBehaviour
{
    [Header("Raycast Settings")]
    [SerializeField] private LayerMask interactionLayer;
    [SerializeField] private float interactionDistance = 5f;

    [Header("UI Hints")]
    [SerializeField] private GameObject hasCrowbarHint; 
    [SerializeField] private GameObject noCrowbarHint; 
    [SerializeField] private GameObject moveChildHint; // Новый текст: "Нажмите ЛКМ, чтобы сдвинуть"

    [Header("Action Settings")]
    [SerializeField] private GameObject objectToDestroy;
    [SerializeField] private GameObject overlayPanel;  
    [SerializeField] private float panelAlpha = 0.9f;
    
    [Header("Child Object Settings")]
    [SerializeField] private GameObject childToMove; // Тот самый дочерний объект
    [SerializeField] private Vector3 targetLocalPosition = new Vector3(0, -0.00425f, 0);

    [Header("Triggers")]
    [SerializeField] private GameObject soundTrigger;   
    [SerializeField] private GameObject darktrigger;  

    private bool isHoveringTarget = false;
    private bool isChildMoved = false;

    private Camera MainCamera => Camera.main;
    private HotbarPanel CurrentHotbar => PersistentObject.Instance != null 
        ? PersistentObject.Instance.GetComponentInChildren<HotbarPanel>(true) 
        : null;

    void Start()
    {
        HideAllUI();
        if(soundTrigger != null) soundTrigger.SetActive(true);
        if(darktrigger != null) darktrigger.SetActive(false);
    }

    void Update()
    {
        HandleRaycast();
    }

    private void HandleRaycast()
    {
        Camera cam = MainCamera;
        if (cam == null) return;

        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, interactionLayer))
        {
            GameObject hitObject = hit.collider.gameObject;

            // 1. Логика для перемещения дочернего объекта
            if (childToMove != null && hitObject == childToMove && !isChildMoved)
            {
                isHoveringTarget = true;
                ShowMoveHint();

                if (Input.GetMouseButtonDown(0) && IsHoldingCrowbar())
                {
                    MoveChildObject();
                }
            }
            // 2. Логика для уничтожения старого объекта (как было раньше)
            else if (objectToDestroy != null && hitObject == objectToDestroy)
            {
                isHoveringTarget = true;
                UpdateInteractionUI();

                if (Input.GetMouseButtonDown(0))
                {
                    TryBreakObject();
                }
            }
        }
        else
        {
            if (isHoveringTarget)
            {
                isHoveringTarget = false;
                HideAllUI();
            }
        }
    }

    private void ShowMoveHint()
    {
        HideAllUI();
        if (IsHoldingCrowbar())
        {
            if (moveChildHint != null) moveChildHint.SetActive(true);
        }
        else
        {
            if (noCrowbarHint != null) noCrowbarHint.SetActive(true);
        }
    }

    private void MoveChildObject()
    {
        childToMove.transform.localPosition = targetLocalPosition;
        isChildMoved = true;
        Debug.Log("Дочерний объект перемещен!");
        HideAllUI();
    }

    private void UpdateInteractionUI()
    {
        if (hasCrowbarHint != null) hasCrowbarHint.SetActive(IsHoldingCrowbar());
        if (noCrowbarHint != null) noCrowbarHint.SetActive(!IsHoldingCrowbar());
    }

    private bool IsHoldingCrowbar()
    {
        HotbarPanel hb = CurrentHotbar;
        if (hb == null) return false;
        ItemScriptableObject selectedItem = hb.GetSelectedItem();
        return selectedItem != null && selectedItem.itemType == ItemType.Crowbar;
    }

    private void TryBreakObject()
    {
        if (IsHoldingCrowbar())
        {
            if (objectToDestroy != null)
            {
                Destroy(objectToDestroy);
                soundTrigger.SetActive(false);
                darktrigger.SetActive(true);
            }

            if (overlayPanel != null)
            {
                overlayPanel.SetActive(true);
                Image panelImage = overlayPanel.GetComponent<Image>();
                if (panelImage != null) panelImage.color = new Color(0, 0, 0, panelAlpha);
            }

            isHoveringTarget = false;
            HideAllUI();
        }
    }

    private void HideAllUI()
    {
        if (hasCrowbarHint != null) hasCrowbarHint.SetActive(false);
        if (noCrowbarHint != null) noCrowbarHint.SetActive(false);
        if (moveChildHint != null) moveChildHint.SetActive(false);
    }
}