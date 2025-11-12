using UnityEngine;
using UnityEngine.InputSystem; // Добавляем using для новой системы ввода

public class PlayerBinds : MonoBehaviour
{
    [Header("Flashlight Settings")]
    public GameObject flashlightObject;
    
    [Header("Input Settings")]
    public InputAction toggleFlashlightAction; // Вместо KeyCode используем InputAction

    private bool isFlashlightOn = false;

    void Start()
    {
        if (flashlightObject == null)
        {
            Debug.LogWarning("Flashlight object not assigned in inspector!");
            return;
        }
        
        flashlightObject.SetActive(false);
        
        // Настраиваем Input Action
        toggleFlashlightAction.performed += ctx => ToggleFlashlight();
        toggleFlashlightAction.Enable();
    }

    void Update()
    {
        // Больше не нужно проверять ввод в Update!
    }

    public void ToggleFlashlight()
    {
        if (flashlightObject == null) return;
        
        isFlashlightOn = !isFlashlightOn;
        flashlightObject.SetActive(isFlashlightOn);
        
        Debug.Log($"Flashlight {(isFlashlightOn ? "ON" : "OFF")}");
    }

    void OnDestroy()
    {
        // Важно: отписываемся и отключаем action при уничтожении объекта
        toggleFlashlightAction.Disable();
        toggleFlashlightAction.performed -= ctx => ToggleFlashlight();
    }

    // Остальные методы остаются без изменений
    public void TurnOnFlashlight()
    {
        if (flashlightObject == null) return;
        isFlashlightOn = true;
        flashlightObject.SetActive(true);
    }

    public void TurnOffFlashlight()
    {
        if (flashlightObject == null) return;
        isFlashlightOn = false;
        flashlightObject.SetActive(false);
    }

    public bool IsFlashlightOn() => isFlashlightOn;
}