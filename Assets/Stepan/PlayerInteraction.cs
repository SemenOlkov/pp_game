using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class PlayerInteraction : MonoBehaviour
{
    public Camera mainCam;
    public float interactionDistance = 10f;
    public GameObject interactionUI;
    public TextMeshProUGUI interactionText;

    public InputActionReference interactActionRef;

    private InputAction interactAction;
    private IInteractable currentInteractable;

    void Awake()
    {
        interactAction = interactActionRef.action;
    }

    private void OnEnable()
    {
        interactAction.Enable();
    }

    private void OnDisable()
    {
        interactAction.Disable();
    }

    void Update()
    {
        InteractionRay();

        if (interactAction.triggered && currentInteractable != null)
        {
            currentInteractable.Interact();
        }
    }

    private void InteractionRay()
    {
        Ray ray = mainCam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        RaycastHit hit;

        bool hitSomething = false;
        currentInteractable = null;

        if (Physics.Raycast(ray, out hit, interactionDistance))
        {
            IInteractable interactable = hit.collider.GetComponent<IInteractable>();
            if (interactable != null)
            {
                hitSomething = true;
                interactionText.text = interactable.GetDescription();
                currentInteractable = interactable;
            }
        }

        interactionUI.SetActive(hitSomething);
    }
}