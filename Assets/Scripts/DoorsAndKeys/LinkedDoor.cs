using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class LinkedDoor : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty interactAction;

    [Header("Door Links")]
    [SerializeField] private LinkedDoor neighborDoor1;
    [SerializeField] private LinkedDoor neighborDoor2;

    [Header("Door State")]
    public bool isOpened = false;
    [SerializeField] private LayerMask doorLayer;
    [SerializeField] private float interactionDistance = 5f;

    [Header("UI Settings")]
    [SerializeField] private GameObject interactionHint;

    [Header("Rotation Settings")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float smoothSpeed = 5f;
    
    private Quaternion closedRotation;
    private Quaternion openRotation;
    private Coroutine animationCoroutine;
    private bool isHovered = false;

    private Camera MainCamera => Camera.main;

    private void OnEnable() => interactAction.action.Enable();
    private void OnDisable() => interactAction.action.Disable();

    void Start()
    {
        closedRotation = transform.localRotation;
        openRotation = closedRotation * Quaternion.Euler(0, openAngle, 0);

        if (interactionHint != null) interactionHint.SetActive(false);
    }

    void Update() => HandleRaycast();

    private void HandleRaycast()
    {
        Ray ray = MainCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        
        if (Physics.Raycast(ray, out RaycastHit hit, interactionDistance, doorLayer))
        {
            if (hit.collider.gameObject == gameObject)
            {
                if (!isHovered)
                {
                    isHovered = true;
                    if (interactionHint != null) interactionHint.SetActive(true);
                }

                if (interactAction.action.WasPressedThisFrame())
                {
                    // Вызываем переключение и разрешаем уведомлять соседей
                    ToggleDoor(true);
                }
                return;
            }
        }

        if (isHovered)
        {
            isHovered = false;
            if (interactionHint != null) interactionHint.SetActive(false);
        }
    }

    // Параметр includeNeighbors предотвращает бесконечный цикл
    public void ToggleDoor(bool includeNeighbors)
    {
        isOpened = !isOpened;

        // Запускаем анимацию
        if (animationCoroutine != null) StopCoroutine(animationCoroutine);
        Quaternion targetRotation = isOpened ? openRotation : closedRotation;
        animationCoroutine = StartCoroutine(AnimateRotation(targetRotation));

        // Если это первичный клик, уведомляем соседей
        if (includeNeighbors)
        {
            // Передаем false, чтобы соседи не начали уведомлять своих соседей в ответ
            if (neighborDoor1 != null) neighborDoor1.ToggleDoor(false);
            if (neighborDoor2 != null) neighborDoor2.ToggleDoor(false);
        }
    }

    private IEnumerator AnimateRotation(Quaternion target)
    {
        while (Quaternion.Angle(transform.localRotation, target) > 0.1f)
        {
            transform.localRotation = Quaternion.Slerp(transform.localRotation, target, Time.deltaTime * smoothSpeed);
            yield return null;
        }
        transform.localRotation = target;
    }
}